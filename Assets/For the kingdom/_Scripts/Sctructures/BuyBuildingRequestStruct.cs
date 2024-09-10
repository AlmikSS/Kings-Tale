[System.Serializable]
public struct BuyBuildingRequestStruct
{
    public ushort PlayerId { get; private set; }
    public ushort BuildingId { get; private set; }

    public BuyBuildingRequestStruct(ushort playerId, ushort buildingId)
    {
        PlayerId = playerId;
        BuildingId = buildingId;
    }
}