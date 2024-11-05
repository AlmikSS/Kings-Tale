using System.Collections.Generic;
using Unity.Netcode;

public class PlayerData 
{
    private ResourcesStruct _resources;
    private List<ulong> _units = new();
    private List<ulong> _buildings = new();
    
    public ResourcesStruct Resources => _resources;
    
    public PlayerData(ResourcesStruct resources)
    {
        _resources = resources;
    }

    public void AddUnit(ulong id)
    {
        if (!_units.Contains(id))
            _units.Add(id);
    }

    public void RemoveUnit(ulong id)
    {
        if (_units.Contains(id))
            _units.Remove(id);
    }
    
    public void AddBuilding(ulong id)
    {
        if (!_buildings.Contains(id))
            _buildings.Add(id);
    }
    
    public void RemoveBuilding(ulong id)
    {
        if (_buildings.Contains(id))
            _buildings.Remove(id);
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
    }
}