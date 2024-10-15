using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class LobbyManager
{
    public delegate void OpenInLobbyMenuDelegate(Lobby lobby, bool isLobbyOwner);
    public event OpenInLobbyMenuDelegate OpenInLobbyMenuEvent;
    public event Action CloseInLobbyMenuEvent;
    
    private Lobby _currentLobby;
    private bool _isTryingToChangeLobby;
    private bool _isLobbyOwner;

    public async void CreateLobbyAsync(int maxPlayers, string gameMode, string playerNickName, bool isPrivate = false)
    {
        if (_isTryingToChangeLobby) return;
        
        _isTryingToChangeLobby = true;
        Debug.Log("Start lobby creating.");
        Allocation allocation = await CreateAllocationAsync(maxPlayers);
        var relayCode = await GetRelayCodeAsync(allocation);
        
        CreateLobbyOptions options = new()
        {
            IsPrivate = isPrivate,
            Player = new()
            {
                Data = new()
                {
                    { StartMenuConstants.PLAYER_NICKNAME, new(PlayerDataObject.VisibilityOptions.Member, playerNickName)}
                }
            },
            Data = new()
            {
                { StartMenuConstants.RELAY_CODE, new(DataObject.VisibilityOptions.Member, relayCode) },
                { StartMenuConstants.GAME_MODE, new(DataObject.VisibilityOptions.Public, gameMode) }
            }
        };

        try
        {
            Debug.Log("Try to create lobby.");
            var lobby = await LobbyService.Instance.CreateLobbyAsync(StartMenuConstants.LOBBY_NAME, maxPlayers, options);
            _currentLobby = lobby;
            HandleLobbyHeartBeatAsync();
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "udp"));
            NetworkManager.Singleton.StartHost();
            _isLobbyOwner = true;
            _isTryingToChangeLobby = false;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            CloseInLobbyMenuEvent?.Invoke();
            return;
        }
        
        Debug.Log("Lobby created.");
        OpenInLobbyMenuEvent?.Invoke(_currentLobby, _isLobbyOwner);
    }

    public async void JoinLobbyAsync(string lobbyCode, string playerNickName)
    {
        if (_isTryingToChangeLobby) return;
        
        _isTryingToChangeLobby = true;
        Debug.Log("Joining lobby by code started. Code: " + lobbyCode);

        JoinLobbyByCodeOptions options = new()
        {
            Player = new()
            {
                Data = new()
                {
                    { StartMenuConstants.PLAYER_NICKNAME, new(PlayerDataObject.VisibilityOptions.Member, playerNickName) }
                }
            }
        };

        try
        {
            Debug.Log("Try to join lobby by code. Code: " + lobbyCode);
            var lobby = await LobbyService.Instance.JoinLobbyByCodeAsync(lobbyCode, options);
            var joinAllocation = await JoinAllocationAsync(lobby.Data[StartMenuConstants.RELAY_CODE].Value);
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "udp"));
            NetworkManager.Singleton.StartClient();
            _currentLobby = lobby;
            _isTryingToChangeLobby = false;
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            CloseInLobbyMenuEvent?.Invoke();
            return;
        }
        
        Debug.Log("Lobby joined.");
        OpenInLobbyMenuEvent?.Invoke(_currentLobby, _isLobbyOwner);
    }

    private async Task<Allocation> CreateAllocationAsync(int maxConnections)
    {
        Debug.Log("Start creating allocation.");
        Allocation allocation;
        try
        {
            Debug.Log("Try to create allocation.");
            allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }

        Debug.Log("Allocation created.");
        return allocation;
    }

    private async Task<string> GetRelayCodeAsync(Allocation allocation)
    {
        Debug.Log("Start getting relay code.");
        string code;
        try
        {
            Debug.Log("Try to get relay code.");
            code = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }

        Debug.Log("Relay code got.");
        return code;
    }

    private async Task<JoinAllocation> JoinAllocationAsync(string relayCode)
    {
        Debug.Log("Start join allocation.");
        JoinAllocation allocation;
        try
        {
            Debug.Log("Try to join allocation.");
            allocation = await RelayService.Instance.JoinAllocationAsync(relayCode);
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
            return default;
        }
        
        Debug.Log("Allocation joined.");
        return allocation;
    }
    
    private async void HandleLobbyHeartBeatAsync()
    {
        while (_currentLobby != null)
        {
            Debug.Log("Heartbeat sent. LobbyID: " + _currentLobby.Id);
            await LobbyService.Instance.SendHeartbeatPingAsync(_currentLobby.Id);
            await Task.Delay(15000);
        }
    }
}