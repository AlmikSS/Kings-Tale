using Unity.Netcode;

public struct ServerAddUnitsPlaceRequestStruct : INetworkSerializable
{
    public ulong PlayerId;

    public ServerAddUnitsPlaceRequestStruct(ulong playerId)
    {
        PlayerId = playerId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref PlayerId);
    }
}