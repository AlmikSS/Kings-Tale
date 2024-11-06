using Unity.Netcode;
using UnityEngine;

public struct ClientBuyResponseStruct : INetworkSerializable
{
    public Vector3 Position;
    public bool CanBuy;
    public ulong PlayerId;
    public ushort Id;

    public ClientBuyResponseStruct(Vector3 position, bool canBuy, ulong playerId, ushort id)
    {
        Position = position;
        CanBuy = canBuy;
        PlayerId = playerId;
        Id = id;
    }

    public ClientBuyResponseStruct(bool canBuy, ulong playerId, ushort id) : this()
    {
        CanBuy = canBuy;
        PlayerId = playerId;
        Id = id;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Position);
        serializer.SerializeValue(ref CanBuy);
        serializer.SerializeValue(ref PlayerId);
        serializer.SerializeValue(ref Id);
    }
}