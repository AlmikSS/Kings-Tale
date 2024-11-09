using Unity.Netcode;

public struct ServerTakeDamageRequestStruct : INetworkSerializable
{
    public ulong PlayerId;
    public ulong Id;
    public uint Damage;

    public ServerTakeDamageRequestStruct(ulong playerId, ulong id, uint damage)
    {
        PlayerId = playerId;
        Id = id;
        Damage = damage;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref PlayerId);
        serializer.SerializeValue(ref Id);
        serializer.SerializeValue(ref Damage);
    }
}