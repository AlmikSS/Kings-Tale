using Unity.Netcode;

public struct ServerSetUnitBuildingRequestStruct : INetworkSerializable
{
    public ulong PlayerId;
    public ulong UnitId;
    public ulong BuildingId;
    public bool IsOwned;

    public ServerSetUnitBuildingRequestStruct(ulong playerId, ulong unitId, ulong buildingId, bool isOwned)
    {
        PlayerId = playerId;
        UnitId = unitId;
        BuildingId = buildingId;
        IsOwned = isOwned;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref PlayerId);
        serializer.SerializeValue(ref UnitId);
        serializer.SerializeValue(ref BuildingId);
        serializer.SerializeValue(ref IsOwned);
    }
}