using Unity.Netcode;

public struct ServerBuyRequestStruct : INetworkSerializable
{
    public ulong PlayerId;
    public ushort Id;
    public bool IsBuilding;

    public ServerBuyRequestStruct(ulong playerId, ushort id, bool isBuilding)
    {
        PlayerId = playerId;
        Id = id;
        IsBuilding = isBuilding;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref PlayerId);
        serializer.SerializeValue(ref Id);
        serializer.SerializeValue(ref IsBuilding);
    }
}