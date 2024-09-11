public struct ServerBuyRequestStruct
{
    public ushort PlayerId { get; private set; }
    public ushort Id { get; private set; } 
    public bool IsBuilding { get; private set; }

    public ServerBuyRequestStruct(ushort playerId, ushort id, bool isBuilding)
    {
        PlayerId = playerId;
        Id = id;
        IsBuilding = isBuilding;
    }
}