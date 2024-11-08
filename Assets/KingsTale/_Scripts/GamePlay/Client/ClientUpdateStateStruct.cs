using Unity.Netcode;

public struct ClientUpdateStateStruct : INetworkSerializable
{
    public ResourcesStruct Resources;
    public ulong[] Units;
    public ulong[] Buildings;

    public ClientUpdateStateStruct(ResourcesStruct resources, ulong[] units, ulong[] buildings)
    {
        Resources = resources;
        Units = units;
        Buildings = buildings;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Resources);
        serializer.SerializeValue(ref Units);
        serializer.SerializeValue(ref Buildings);
    }
}