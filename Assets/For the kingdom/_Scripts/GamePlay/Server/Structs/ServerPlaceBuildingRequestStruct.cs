using Unity.Netcode;

public struct ServerPlaceBuildingRequestStruct : INetworkSerializable
{
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        throw new System.NotImplementedException();
    }
}