using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameData : NetworkBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private List<NetworkObject> _unitsPrefabs = new();
    [SerializeField] private List<NetworkObject> _buildingsPrefabs = new();
    [SerializeField] private NetworkObject _playerObject;

    [Header("Configs/Units")]
    [SerializeField] private List<UnitAttackConfigSO> _attackUnitsConfigs = new();
    [SerializeField] private UnitWorkerConfigSO _workerUnitConfig;
    
    private readonly Dictionary<ulong, PlayerData> _players = new();
    private List<ulong> _units = new();
    private List<ulong> _buildings = new();
    
    public void Initialize()
    {
        GetConfigs();
    }
    
    public void RegisterClient(ulong clientId)
    {
        if (!IsServer) return;
        
        var newObj = Instantiate(_playerObject);
        newObj.SpawnAsPlayerObject(clientId);
        var playerManager = newObj.GetComponent<PlayerManager>();
        
        var playerData = new PlayerData(new ResourcesStruct(), playerManager);
        _players.Add(clientId, playerData);
        Debug.Log("Registering" + clientId);
    }
    
    public PlayerData GetPlayer(ulong id)
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

    public void AddUnit(ulong id, ulong playerId)
    {
        if (!_units.Contains(id))
        {
            _units.Add(id);
            _players[playerId].AddUnit(id);
        }
    }

    public void RemoveUnit(ulong id, ulong playerId)
    {
        if (_units.Contains(id))
        {
            _units.Remove(id);
            _players[playerId].RemoveUnit(id);
        }
    }
    
    public void AddBuilding(ulong id, ulong playerId)
    {
        if (!_buildings.Contains(id))
        {
            _buildings.Add(id);
            _players[playerId].AddBuilding(id);
        }
    }
    
    public void RemoveBuilding(ulong id, ulong playerId)
    {
        if (_buildings.Contains(id))
        {
            _buildings.Remove(id);
            _players[playerId].RemoveBuilding(id);
        }
    }
        
    private void GetConfigs()
    {
        
    }

    public ResourcesStruct? GetPrice(uint id, bool isBuilding)
    {
        return default;
    }

    public NetworkObject GetBuilding(ushort id)
    {
        foreach (var building in _buildingsPrefabs)
        {
            if (building.GetComponent<Building>().Id == id)
            return building;
        }

        return null;
    }

    public NetworkObject GetUnit(ushort id)
    {
        return _unitsPrefabs[id];
    }
}