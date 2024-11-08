using UnityEngine;

public class UnitBaseConfigSO : ScriptableObject
{
    [SerializeField] private string _unitName;
    
    [Header("Health")]
    [SerializeField] private uint _maxHealth;
    [SerializeField] private uint _regeneration;
    [SerializeField] private float _regenSpeed;
    
    [Header("Resists")]
    [SerializeField] private uint _magicResist;
    [SerializeField] private uint _physicalResist;

    [Header("Movement")]
    [SerializeField] private float _speed;
    [SerializeField] private float _angularSpeed;
    [SerializeField] private float _stopDistance;

    [Header("Price")]
    [SerializeField] private ResourcesStruct _price;
    
    public string UnitName => _unitName;

    public uint MaxHealth => _maxHealth;
    public uint Regeneration => _regeneration;
    public float RegenSpeed => _regenSpeed;
    
    public uint MagicResist => _magicResist;
    public uint PhysicalResist => _physicalResist;
    
    public float Speed => _speed;
    public float AngularSpeed => _angularSpeed;
    public float StopDistance => _stopDistance;

    public ResourcesStruct Price => _price;

    public void SetProperties(uint maxHealth, uint regeneration, float regenSpeed, uint magicResist, uint physicalResist, float speed, float angularSpeed, float stopDistance, ResourcesStruct price)
    {
        _maxHealth = maxHealth;
        _regeneration = regeneration;
        _regenSpeed = regenSpeed;
        
        _magicResist = magicResist;
        _physicalResist = physicalResist;
        
        _speed = speed;
        _angularSpeed = angularSpeed;
        _stopDistance = stopDistance;

        _price = price;
    }
}