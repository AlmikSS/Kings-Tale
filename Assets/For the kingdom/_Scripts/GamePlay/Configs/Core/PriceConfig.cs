[System.Serializable]
public class PriceConfig
{
    public readonly int FoodPrice;
    public readonly int WoodPrice;
    public readonly int GoldPrice;
    
    public PriceConfig(IGameConfig gameConfig)
    {
        FoodPrice = gameConfig.FoodPrice;
        WoodPrice = gameConfig.WoodPrice;
        GoldPrice = gameConfig.GoldPrice;
    }
}