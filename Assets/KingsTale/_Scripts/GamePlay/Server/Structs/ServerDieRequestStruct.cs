using Unity.Netcode;

public struct ServerDieRequestStruct : INetworkSerializable
{
    public ulong Id;
    public bool IsBuilding;

    public ServerDieRequestStruct(ulong id, bool isBuilding)
    {
        Id = id;
        IsBuilding = isBuilding;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Id);
        serializer.SerializeValue(ref IsBuilding);
    }
}