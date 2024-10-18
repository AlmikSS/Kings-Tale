using Unity.Netcode;
using UnityEngine;

public struct ServerBuyRequestStruct : INetworkSerializable
{
    public Vector3 Position;
    public ulong PlayerId;
    public ushort Id;
    public bool IsBuilding;
    
    public ServerBuyRequestStruct(Vector3 position, ulong playerId, ushort id, bool isBuilding)
    {
        Position = position;
        PlayerId = playerId;
        Id = id;
        IsBuilding = isBuilding;
    }

    public ServerBuyRequestStruct(ulong playerId, ushort id, bool isBuilding) : this()
    {
        PlayerId = playerId;
        Id = id;
        IsBuilding = isBuilding;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref PlayerId);
        serializer.SerializeValue(ref Id);
        serializer.SerializeValue(ref IsBuilding);
        serializer.SerializeValue(ref Position);
    }
}