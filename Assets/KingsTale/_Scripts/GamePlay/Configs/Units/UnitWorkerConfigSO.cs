using UnityEngine;

[CreateAssetMenu(fileName = "NewWorkerUnitConfig", menuName = "Configs/Units/WorkerUnit")]
public class UnitWorkerConfigSO : UnitBaseConfigSO
{
    [Header("Work options")]
    [SerializeField] private float _attackSpeed;
    
    public float AttackSpeed => _attackSpeed;
    
    public void SetProperties(uint maxHealth, uint regeneration, float regenSpeed, uint magicResist, uint physicalResist, float speed, float angularSpeed, float stopDistance, float attackSpeed)
    {
        base.SetProperties(maxHealth, regeneration, regenSpeed, magicResist, physicalResist, speed, angularSpeed, stopDistance);
        _attackSpeed = attackSpeed;
    }
}