public class PlayerData
{
    public uint Wood { get; private set; }
    public uint Gold { get; private set; }
    public uint Food { get; private set; }

    public PlayerData(uint wood, uint gold, uint food)
    {
        Wood = wood;
        Gold = gold;
        Food = food;
    }
}