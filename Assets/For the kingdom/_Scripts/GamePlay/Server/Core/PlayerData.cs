using Unity.Netcode;

public class PlayerData : INetworkSerializable
{
    private ResourcesStruct _resources;
    private NetworkList<ulong> _units = new();
    
    public ResourcesStruct Resources => _resources;
    
    public PlayerData(ResourcesStruct resources)
    {
        _resources = resources;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref _resources);
    }

    public void AddUnit(ulong id)
    {
        _units.Add(id);
    }

    public NetworkObject GetUnit(ulong id)
    {
        if (!_units.Contains(id))
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