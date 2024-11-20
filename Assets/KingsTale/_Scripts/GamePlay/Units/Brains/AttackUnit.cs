using System.Collections;
using Unity.Netcode;
using UnityEngine;

public abstract class AttackUnit : UnitBrain
{
    [SerializeField] private LayerMask _attackableLayerMask;

    private NetworkObject _target;
    private bool _isLongRange;
    private uint _damage;
    private float _attackSpeed;
    private float _startAttackDistance;
    private float _visionRange;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;

        base.OnNetworkSpawn();

        _damage = ((UnitAttackConfigSO)_config).Damage;
        _attackSpeed = ((UnitAttackConfigSO)_config).AttackSpeed;
        _startAttackDistance = ((UnitAttackConfigSO)_config).StartAttackDistance;
        _visionRange = ((UnitAttackConfigSO)_config).VisionRange;
        _isLongRange = ((UnitAttackConfigSO)_config).IsLongRange;

        StartCoroutine(LiveCycleRoutine());
    }

    private IEnumerator LiveCycleRoutine()
    {
        while (true)
        {
            if (_target != null)
            {
                if (Vector3.Distance(transform.position, _target.transform.position) < _startAttackDistance)
                {
                    _agent.isStopped = true;
                    _agent.path = null;
                    Attack();
                    yield return new WaitForSeconds(_attackSpeed);
                }
                else
                    GoToPoint(_target.transform.position);
            }
        }
    }

    private void Attack()
    {
        Debug.Log("Attack!!");
    }
    
    private void SeekEnemy()
    {
        var cols = Physics.OverlapSphere(transform.position, _visionRange, _attackableLayerMask);

        foreach (var col in cols)
        {
            if (col.gameObject.TryGetComponent(out UnitBrain brain))
            {
                if (brain.OwnerClientId != OwnerClientId)
                {
                    _target = brain.NetworkObject;
                }
            }
        }
    }

    [Rpc(SendTo.Owner)]
    public override void SetBuildingRpc(ulong buildingID)
    {
        var building = NetworkManager.Singleton.SpawnManager.SpawnedObjects[buildingID];
        
        GoToPoint(building.transform.position);
        
        if (building.OwnerClientId != OwnerClientId)
            _target = building;
    }
}