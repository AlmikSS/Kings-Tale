using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody), typeof(NavMeshObstacle))]
public abstract class Building : NetworkBehaviour, IDamagable
{
    [SerializeField] protected BuildingBaseConfigSO _config;
    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField] private ushort _id;
    [SerializeField] private HealthSlider _healthSlider;

    public int Health => _currentHealth.Value;
    public bool CanBuild { get; private set; } = true;
    public bool IsBuilt => _isBuilt.Value;
    public ushort Id => _id;
 
    protected NetworkVariable<bool> _isPlaced = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    protected NetworkVariable<bool> _isBuilt = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    protected NetworkVariable<int> _currentHealth = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    protected int _magicResist;
    protected int _physicalResist;
    protected float _buildTime;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }

        GetComponentInChildren<MeshRenderer>().sharedMaterial.color = new Color(1, 1, 1, 0.3f);

        _id = _config.Id;
        _currentHealth.Value = (int)_config.MaxHealth;
        _magicResist = (int)_config.MagicResist;
        _physicalResist = (int)_config.PhysicalResist;
        _buildTime = _config.BuildTime;
        
        _healthSlider.SetMaxHealth(_currentHealth.Value);
    }
    
    private void OnTriggerStay(Collider other)
    {
        {
            if (other.CompareTag(GamePlayConstants.BUILDING_TAG))
                CanBuild = false;
        }
    }
    
    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(GamePlayConstants.BUILDING_TAG))
            CanBuild = true;
    }

    [Rpc(SendTo.Owner)]
    public virtual void PlaceBuildingRpc()
    {
        _isPlaced.Value = true;
    }

    public WorkClass BuildBuilding()
    {
        WorkClass work = new();

        List<WorkerActionStruct> actions = new();

        var firstAction = new WorkerActionStruct
        {
            Action = WorkerAction.GoToPoint,
            Target = NetworkObject
        };

        var secondAction = new WorkerActionStruct
        {
            Action = WorkerAction.Wait,
            Target = NetworkObject,
            WaitTime = _buildTime,
            WithAction = true
        };
        
        actions.Add(firstAction);
        actions.Add(secondAction);

        work.Actions = actions;
        return work;
    }

    [Rpc(SendTo.Owner)]
    public virtual void BuildRpc()
    {
        _isBuilt.Value = true;
        GetComponentInChildren<MeshRenderer>().sharedMaterial.color = Color.white;
    }
    
    [Rpc(SendTo.Owner)]
    public virtual void TakeDamageRpc(int damage)
    {
        if (damage > 0)
            _currentHealth.Value -= damage;
        
        _healthSlider.TakeDamage(damage);
        
        if (_currentHealth.Value > 0) return;

        Die();
    }
    
    public virtual void Die()
    {
        Invoke(nameof(DieRpc), 0.5f);
    }
    
    [Rpc(SendTo.Server)]
    private void DieRpc()
    {
        var request = new ServerDieRequestStruct
        {
            Id = NetworkObjectId,
            IsBuilding = false
        };
		
        InputManager.Instance.HandleDieRequestRpc(request);
    }
}

