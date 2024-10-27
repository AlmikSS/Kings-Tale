using Unity.Netcode;

public struct PlayerDataStruct : INetworkSerializable
{
    public ResourcesStruct Resources;

    public PlayerDataStruct(ResourcesStruct resources)
    {
        Resources = resources;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Resources);
    }
}