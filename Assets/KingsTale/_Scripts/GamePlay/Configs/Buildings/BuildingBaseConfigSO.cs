using UnityEngine;
using UnityEngine.Localization;

public abstract class BuildingBaseConfigSO : ScriptableObject
{
    [SerializeField] private ushort _id;

    [Header("Naming")]
    [SerializeField] private LocalizedString _name;
    [SerializeField] private LocalizedString _description;

    [Header("Health")]
    [SerializeField] private uint _maxHealth;
    
    [Header("Resists")]
    [SerializeField] private uint _magicResist;
    [SerializeField] private uint _physicalResist;
    
    [Header("Price")]
    [SerializeField] private ResourcesStruct _price;

    [SerializeField] private float _buildTime;

    public ushort Id => _id;
    public LocalizedString Name => _name;

    public LocalizedString Description => _description;

    public uint MaxHealth => _maxHealth;
    
    public uint MagicResist => _magicResist;
    public uint PhysicalResist => _physicalResist;
    
    public ResourcesStruct Price => _price;
    
    public float BuildTime => _buildTime;
    
    public void SetProperties(uint maxHealth, uint magicResist, uint physicalResist, ResourcesStruct price, float buildTime)
    {
        _maxHealth = maxHealth;
        
        _magicResist = magicResist;
        _physicalResist = physicalResist;

        _price = price;

        _buildTime = buildTime;
    }
}