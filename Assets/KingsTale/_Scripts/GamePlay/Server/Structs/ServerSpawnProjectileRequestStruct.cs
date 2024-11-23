using Unity.Netcode;
using UnityEngine;

public struct ServerSpawnProjectileRequestStruct : INetworkSerializable
{
    public ulong Id;
    public ushort ProjectileId;
    public Vector3 Position;

    public ServerSpawnProjectileRequestStruct(ulong id, ushort projectileId, Vector3 position)
    {
        Id = id;
        ProjectileId = projectileId;
        Position = position;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Id);
        serializer.SerializeValue(ref ProjectileId);
        serializer.SerializeValue(ref Position);
    }
}