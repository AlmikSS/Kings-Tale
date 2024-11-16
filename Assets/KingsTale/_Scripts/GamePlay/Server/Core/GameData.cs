using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameData : NetworkBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private List<NetworkObject> _unitsPrefabs = new();
    [SerializeField] private List<NetworkObject> _buildingsPrefabs = new();
    [SerializeField] private NetworkObject _playerObject;
    [SerializeField] private NetworkObject _mainBuildingPrefab;

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

        var pos = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
        var newObj = Instantiate(_playerObject, pos, _playerObject.transform.rotation);
        var mainBuilding = Instantiate(_mainBuildingPrefab, pos, _mainBuildingPrefab.transform.rotation);
        newObj.SpawnAsPlayerObject(clientId);
        mainBuilding.SpawnWithOwnership(clientId);
        var playerManager = newObj.GetComponent<PlayerManager>();
        playerManager.GetComponent<BuildingSystem>().SetBuildingList(_buildingsPrefabs);
        playerManager.SetMainBuildingRpc(mainBuilding);
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

    public ResourcesStruct GetPlayerResources(ulong id)
    {
        if (_players.TryGetValue(id, out var player))
            return player.Resources;
        
        return default;
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

    public ResourcesStruct GetPrice(ushort id, bool isBuilding)
    {
        if (!isBuilding)
        {
            if (id == 0)
                return _workerUnitConfig.Price;
            
            foreach (var unit in _attackUnitsConfigs)
            {
                if (unit.UnitId == id)
                    return unit.Price;
            }
        }

        return default;
    }

    public NetworkObject GetBuildingPrefab(ushort id)
    {
        foreach (var building in _buildingsPrefabs)
        {
            if (building.GetComponent<Building>().Id == id)
                return building;
        }
        
        return null;
    }

    public NetworkObject GetUnitPrefab(ushort id)
    {
        foreach (var unit in _unitsPrefabs)
        {
            if (unit.GetComponent<UnitBrain>().Id == id)
                return unit;
        }
        
        return _unitsPrefabs[id];
    }

    public bool IsUnitExist(ulong id)
    {
        return _units.Contains(id);
    }
}