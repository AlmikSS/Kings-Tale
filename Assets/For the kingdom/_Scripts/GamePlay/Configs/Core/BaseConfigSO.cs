using UnityEngine;

public class BaseConfigSO : ScriptableObject, IGameConfig
{
    [SerializeField] private int _woodPrice;
    [SerializeField] private int _goldPrice;
    [SerializeField] private int _foodPrice;
    
    public int WoodPrice { get => _woodPrice; }
    public int GoldPrice { get => _goldPrice; }
    public int FoodPrice { get => _foodPrice; }
}