public struct BuyRequestStruct
{
    public ushort PlayerId { get; private set;  }
    public bool IsBuilding { get; private set; }
    public ushort Id { get; private set; }

    public BuyRequestStruct(ushort playerId, bool isBuilding, ushort id)
    {
        PlayerId = playerId;
        IsBuilding = isBuilding;
        Id = id;
    }
}