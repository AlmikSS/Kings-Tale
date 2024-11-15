using Unity.Netcode;

public struct ServerAddResourcesRequestStruct : INetworkSerializable
{
    public ResourcesStruct ResourcesToAdd;
    public ulong PlayerId;

    public ServerAddResourcesRequestStruct(ResourcesStruct resourcesToAdd, ulong playerId)
    {
        ResourcesToAdd = resourcesToAdd;
        PlayerId = playerId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref ResourcesToAdd);
        serializer.SerializeValue(ref PlayerId);
    }
}