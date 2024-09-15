public struct PlayerDataStruct
{
    public int Wood { get; private set; }
    public int Gold { get; private set; }
    public int Food { get; private set; }

    public PlayerDataStruct(int wood, int gold, int food)
    {
        Wood = wood;
        Gold = gold;
        Food = food;
    }
}