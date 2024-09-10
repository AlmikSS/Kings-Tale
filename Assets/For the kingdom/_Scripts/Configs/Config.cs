public abstract class Config
{
    public uint WoodPrice { get; private set; }
    public uint GoldPrice { get; private set; }
    public uint FoodPrice { get; private set; }

    public Config(uint woodPrice, uint goldPrice, uint foodPrice)
    {
        WoodPrice = woodPrice;
        GoldPrice = goldPrice;
        FoodPrice = foodPrice;
    }
}