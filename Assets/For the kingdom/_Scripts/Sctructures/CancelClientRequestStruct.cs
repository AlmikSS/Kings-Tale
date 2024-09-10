[System.Serializable]
public struct CancelClientRequestStruct
{
    public ushort PlayerId { get; private set; }
    public string Reason { get; private set; }

    public CancelClientRequestStruct(ushort playerId, string reason = "")
    {
        PlayerId = playerId;
        Reason = reason;
    }
}