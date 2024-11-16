using UnityEngine;

public abstract class BuildingBaseConfigSO : ScriptableObject
{
    [SerializeField] private ushort _id;
    
    [Header("Health")]
    [SerializeField] private uint _maxHealth;
    
    [Header("Resists")]
    [SerializeField] private uint _magicResist;
    [SerializeField] private uint _physicalResist;
    
    [Header("Price")]
    [SerializeField] private ResourcesStruct _price;
    
    public ushort Id => _id;

    public uint MaxHealth => _maxHealth;
    
    public uint MagicResist => _magicResist;
    public uint PhysicalResist => _physicalResist;
    
    public ResourcesStruct Price => _price;
    
    public void SetProperties(uint maxHealth, uint magicResist, uint physicalResist, ResourcesStruct price)
    {
        _maxHealth = maxHealth;
        
        _magicResist = magicResist;
        _physicalResist = physicalResist;

        _price = price;
    }
}