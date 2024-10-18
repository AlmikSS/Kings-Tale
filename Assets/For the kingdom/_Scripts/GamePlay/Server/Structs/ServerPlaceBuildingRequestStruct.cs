using Unity.Netcode;
using UnityEngine;

public struct ServerPlaceBuildingRequestStruct : INetworkSerializable
{
    public Vector3 Position;
    public ulong PlayerId;
    public ushort BuildingId;

    public ServerPlaceBuildingRequestStruct(Vector3 position, ulong playerId, ushort buildingId)
    {
        Position = position;
        PlayerId = playerId;
        BuildingId = buildingId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Position);
        serializer.SerializeValue(ref PlayerId);
        serializer.SerializeValue(ref BuildingId);
    }
}