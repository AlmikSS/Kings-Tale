public struct BuyUnitRequestStruct
{
    public ushort PlayerId { get; private set; }
    public ushort UnitId { get; private set; }

    public BuyUnitRequestStruct(ushort playerId, ushort unitId)
    {
        PlayerId = playerId;
        UnitId = unitId;
    }
}