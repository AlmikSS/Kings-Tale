using System.Collections.Generic;
using Unity.Netcode;

public class PlayerData
{
    private PlayerManager _playerManager;
    private ResourcesStruct _resources;
    private List<ulong> _units = new();
    private List<ulong> _buildings = new();
    
    public ResourcesStruct Resources => _resources;
    public PlayerManager PlayerManager => _playerManager;
    
    public PlayerData(ResourcesStruct resources, PlayerManager playerManager)
    {
        _resources = resources;
        _playerManager = playerManager;
    }

    private void UpdatePlayer()
    {
        var updatedState = new ClientUpdateStateStruct
        {
            Resources = _resources,
            Units = _units.ToArray(),
            Buildings = _buildings.ToArray()
        };
        _playerManager.UpdateStateRpc(updatedState);
    }
    
    public void AddUnit(ulong id)
    {
        if (!_units.Contains(id))
            _units.Add(id);

        UpdatePlayer();
    }

    public void RemoveUnit(ulong id)
    {
        if (_units.Contains(id))
            _units.Remove(id);

        UpdatePlayer();
    }
    
    public void AddBuilding(ulong id)
    {
        if (!_buildings.Contains(id))
            _buildings.Add(id);
        
        UpdatePlayer();
    }
    
    public void RemoveBuilding(ulong id)
    {
        if (_buildings.Contains(id))
            _buildings.Remove(id);
        
        UpdatePlayer();
    }
    
    public NetworkObject GetUnit(ulong id)
    {
        if (!_units.Contains(id))
            return default;
            
        return NetworkManager.Singleton.SpawnManager.SpawnedObjects[id];
    }

    public NetworkObject GetBuilding(ulong id)
    {
        if (!_buildings.Contains(id))
            return default;
            
        return NetworkManager.Singleton.SpawnManager.SpawnedObjects[id];
    }
    
    public void ChangeResources(ResourcesStruct resources, ushort op)
    {
        if (op == 0)
        {
            _resources.Wood += resources.Wood;
            _resources.Food += resources.Food;
            _resources.Gold += resources.Gold;
        }
        else if (op == 1)
        {
            _resources.Wood -= resources.Wood;
            _resources.Food -= resources.Food;
            _resources.Gold -= resources.Gold;
        }
        
        UpdatePlayer();
    }
}