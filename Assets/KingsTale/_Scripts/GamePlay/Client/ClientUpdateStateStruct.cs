using Unity.Netcode;

public struct ClientUpdateStateStruct : INetworkSerializable
{
    public ResourcesStruct Resources;
    public ulong[] Units;
    public ulong[] Buildings;
    public uint MaxUnitsCount;

    public ClientUpdateStateStruct(ResourcesStruct resources, ulong[] units, ulong[] buildings, uint maxUnitsCount)
    {
        Resources = resources;
        Units = units;
        Buildings = buildings;
        MaxUnitsCount = maxUnitsCount;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Resources);
        serializer.SerializeValue(ref Units);
        serializer.SerializeValue(ref Buildings);
        serializer.SerializeValue(ref MaxUnitsCount);
    }
}