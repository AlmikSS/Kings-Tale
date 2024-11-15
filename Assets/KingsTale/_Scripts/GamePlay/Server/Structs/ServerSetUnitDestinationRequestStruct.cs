using Unity.Netcode;
using UnityEngine;

public struct ServerSetUnitDestinationRequestStruct : INetworkSerializable
{
    public Vector3 Point;
    public ulong PlayerId;
    public ulong UnitId;

    public ServerSetUnitDestinationRequestStruct(Vector3 point, ulong playerId, ulong unitId)
    {
        Point = point;
        PlayerId = playerId;
        UnitId = unitId;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Point);
        serializer.SerializeValue(ref PlayerId);
        serializer.SerializeValue(ref UnitId);
    }
}