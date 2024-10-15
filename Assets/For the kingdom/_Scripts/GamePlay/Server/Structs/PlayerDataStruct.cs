using Unity.Netcode;

public struct PlayerDataStruct : INetworkSerializable
{
    public int Wood;
    public int Gold;
    public int Food;

    public PlayerDataStruct(int wood, int gold, int food)
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