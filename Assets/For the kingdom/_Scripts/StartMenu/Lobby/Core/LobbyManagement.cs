using System;
using System.Threading.Tasks;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class LobbyManagement
{
    public event Action<Lobby, bool> OnLobbyUpdateEvent; 
    
    private Lobby _currentLobby;
    private bool _isLobbyOwner;

    public void SetLobby(Lobby lobby, bool isLobbyOwner)
    {
        _currentLobby = lobby;
        _isLobbyOwner = isLobbyOwner;
        UpdateLobbyStateAsync();
    }

    public void DeleteLobby() => _currentLobby = null;
    
    public async void KickPlayerAsync(string playerId)
    {
        try
        {
            Debug.Log("Try to kick player.");
            await LobbyService.Instance.RemovePlayerAsync(_currentLobby.Id, playerId);
        }
        catch (LobbyServiceException e)
        {
            Debug.Log(e);
            return;
        }
        
        Debug.Log($"Player {playerId} kicked.");
    }

    private async void UpdateLobbyStateAsync()
    {
        while (_currentLobby != null)
        {
            Debug.Log("Lobby updated.");
            _currentLobby = await LobbyService.Instance.GetLobbyAsync(_currentLobby.Id);
            OnLobbyUpdateEvent?.Invoke(_currentLobby, _isLobbyOwner);
            await Task.Delay(1000);
        }
    }
}