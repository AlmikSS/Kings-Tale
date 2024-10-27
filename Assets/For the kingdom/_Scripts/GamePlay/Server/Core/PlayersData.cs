using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayersData : NetworkBehaviour
{
    [SerializeField] private GameObject _playerObject; 
    
    private Dictionary<ulong, PlayerDataStruct> _players = new();

    public void RegisterClient(ulong clientId)
    {
        if (!IsServer) return;
        
        PlayerDataStruct playerData = new(new ResourcesStruct());
        var newObj = Instantiate(_playerObject).GetComponent<NetworkObject>();
        newObj.SpawnAsPlayerObject(clientId);
        _players.Add(clientId, playerData);
        Debug.Log("Registering" + clientId);
    }
    
    public PlayerDataStruct? GetPlayer(ulong id)
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
            player.Resources.Wood += resourcesToAdd.Wood;
            player.Resources.Gold += resourcesToAdd.Gold;
            player.Resources.Food += resourcesToAdd.Food;
        }
    }

    public void RemoveResourcesToPlayer(ulong playerId, ResourcesStruct resourcesToRemove)
    {
        if (_players.TryGetValue(playerId, out var player))
        {
            player.Resources.Wood -= resourcesToRemove.Wood;
            player.Resources.Gold -= resourcesToRemove.Gold;
            player.Resources.Food -= resourcesToRemove.Food;
        }
    }
}