using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackUnitConfig", menuName = "Configs/Units/AttackUnit")]
public class UnitAttackConfigSO : UnitBaseConfigSO
{
    [Header("Attack options")]
    [SerializeField] private uint _damage;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private float _startAttackDistance;
    [SerializeField] private float _visionRange;
    [SerializeField] private bool _isLongRange;

    public uint Damage => _damage;
    public float AttackSpeed => _attackSpeed;
    public float StartAttackDistance => _startAttackDistance;
    public bool IsLongRange => _isLongRange;
    public float VisionRange => _visionRange;

    public void SetProperties(uint maxHealth, uint regeneration, float regenSpeed, uint magicResist, uint physicalResist, float speed, float angularSpeed, float stopDistance, uint damage, float attackSpeed, bool isLongRange, ResourcesStruct price, float visionRange, float startAttackDistance)
    {
        base.SetProperties(maxHealth, regeneration, regenSpeed, magicResist, physicalResist, speed, angularSpeed, stopDistance, price);
        
        _damage = damage;
        _attackSpeed = attackSpeed;
        _isLongRange = isLongRange;
        _visionRange = visionRange;
        _startAttackDistance = startAttackDistance;
    }
}