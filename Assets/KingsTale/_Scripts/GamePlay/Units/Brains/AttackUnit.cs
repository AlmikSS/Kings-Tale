using System.Collections;
using Unity.Netcode;
using UnityEngine;

public abstract class AttackUnit : UnitBrain
{
    [SerializeField] private LayerMask _attackableLayerMask;
    [SerializeField] private float _stateMachineUpdateDelay;

    protected NetworkObject _target;
    private AttackUnitState _currentState;
    protected uint _damage;
    protected float _attackSpeed;
    protected float _startAttackDistance;
    private float _visionRange;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        if (!IsOwner) return;

        _damage = ((UnitAttackConfigSO)_config).Damage;
        _attackSpeed = ((UnitAttackConfigSO)_config).AttackSpeed;
        _startAttackDistance = ((UnitAttackConfigSO)_config).StartAttackDistance;
        _visionRange = ((UnitAttackConfigSO)_config).VisionRange;

        StartCoroutine(StateMachineRoutine());
    }

    private IEnumerator StateMachineRoutine()
    {
        while (true)
        {
            switch (_currentState)
            {
                case AttackUnitState.Passive:
                    SearchForTarget();
                    break;
                case AttackUnitState.Aggressive:
                    yield return HandleAggressiveBehaviourRoutine();
                    break;
                case AttackUnitState.Die:
                    _target = null;
                    _agent.isStopped = true;
                    break;
            }
            
            if (_stateMachineUpdateDelay > 0)
                yield return new WaitForSeconds(_stateMachineUpdateDelay);
            else
                yield return null;
        }
    }

    private void SearchForTarget()
    {
        var cols = Physics.OverlapSphere(transform.position, _visionRange, _attackableLayerMask);

        foreach (var col in cols)
        {
            if (col.gameObject.TryGetComponent(out UnitBrain unit))
            {
                if (unit.OwnerClientId != OwnerClientId)
                {
                    _target = unit.NetworkObject;
                    _currentState = AttackUnitState.Aggressive;
                }
            }
        }
    }

    private IEnumerator HandleAggressiveBehaviourRoutine()
    {
        if (_target == null || !_target.IsSpawned)
        {
            _currentState = AttackUnitState.Passive;
            yield break;
        }
        
        var distanceToTarget = Vector3.Distance(transform.position, _target.transform.position);

        if (distanceToTarget > _startAttackDistance)
            GoToPoint(_target.transform.position);
        else
        {
            _agent.ResetPath();
            yield return AttackTargetRoutine();
        }
    }

    protected abstract IEnumerator AttackTargetRoutine();
    
    [Rpc(SendTo.Owner)]
    public override void SetBuildingRpc(ulong buildingID)
    {
        var building = NetworkManager.Singleton.SpawnManager.SpawnedObjects[buildingID];
        
        GoToPoint(building.transform.position);
        
        if (building.OwnerClientId != OwnerClientId)
        {
            _target = building;
            _currentState = AttackUnitState.Aggressive;
        }
    }

    public override void TakeDamageRpc(int damage)
    {
        base.TakeDamageRpc(damage);
        
        if (_target == null)
            SearchForTarget();
    }

    public override void Die()
    {
        _currentState = AttackUnitState.Die;
        
        base.Die();
    }
}

public enum AttackUnitState
{
    Passive,
    Aggressive,
    Die
}