using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayersData : NetworkBehaviour
{
    [SerializeField] private GameObject _playerObject; 
    
    private Dictionary<ulong, PlayerData> _players = new();

    public void RegisterClient(ulong clientId)
    {
        if (!IsServer) return;
        
        PlayerData playerData = new(new ResourcesStruct());
        var newObj = Instantiate(_playerObject).GetComponent<NetworkObject>();
        newObj.SpawnAsPlayerObject(clientId);
        _players.Add(clientId, playerData);
        Debug.Log("Registering" + clientId);
    }
    
    public PlayerData? GetPlayer(ulong id)
    {
        if (_players.TryGetValue(id, out var player))
            return player;
        
        return null;
    }

    public ResourcesStruct? GetPlayerResources(ulong id)
    {
        if (_players.TryGetValue(id, out var player))
            return player.Resources;
        
        return null;
    }

    public void AddResourcesToPlayer(ulong playerId, ResourcesStruct resourcesToAdd)
    {
        if (_players.TryGetValue(playerId, out var player))
        {
            player.ChangeResources(resourcesToAdd, 0);
        }
    }

    public void RemoveResourcesToPlayer(ulong playerId, ResourcesStruct resourcesToRemove)
    {
        if (_players.TryGetValue(playerId, out var player))
        {
            player.ChangeResources(resourcesToRemove, 1);
        }
    }
}