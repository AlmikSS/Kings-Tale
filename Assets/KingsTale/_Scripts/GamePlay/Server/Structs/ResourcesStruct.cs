using Unity.Netcode;

[System.Serializable]
public struct ResourcesStruct : INetworkSerializable
{
    public uint Wood;
    public uint Gold;
    public uint Food;
    
    public ResourcesStruct(uint wood, uint gold, uint food)
    {
        Wood = wood;
        Gold = gold;
        Food = food;
    }
    
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Wood);
        serializer.SerializeValue(ref Gold);
        serializer.SerializeValue(ref Food);
    }
}