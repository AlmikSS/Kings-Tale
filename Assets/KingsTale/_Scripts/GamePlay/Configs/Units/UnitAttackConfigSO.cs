using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackUnitConfig", menuName = "Configs/Units/AttackUnit")]
public class UnitAttackConfigSO : UnitBaseConfigSO
{
    [Header("Attack options")]
    [SerializeField] private uint _damage;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private bool _isLongRange;

    public uint Damage => _damage;
    public float AttackSpeed => _attackSpeed;
    public bool IsLongRange => _isLongRange;

    public void SetProperties(uint maxHealth, uint regeneration, float regenSpeed, uint magicResist, uint physicalResist, float speed, float angularSpeed, float stopDistance, uint damage, float attackSpeed, bool isLongRange, ResourcesStruct price)
    {
        base.SetProperties(maxHealth, regeneration, regenSpeed, magicResist, physicalResist, speed, angularSpeed, stopDistance, price);
        _damage = damage;
        _attackSpeed = attackSpeed;
        _isLongRange = isLongRange;
    }
}