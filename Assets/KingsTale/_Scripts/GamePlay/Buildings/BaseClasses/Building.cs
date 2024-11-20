using System.Collections.Generic;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Rigidbody), typeof(NavMeshObstacle))]
public abstract class Building : NetworkBehaviour, IDamagable
{
    [HideInInspector] public List<Effect> Effects = new();

    [SerializeField] protected BuildingBaseConfigSO _config;
    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField] private ushort _id;
    [SerializeField] private HealthSlider _healthSlider;

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
    
    public IEnumerator ChangeParam(Effect name, int value, float duration, bool increase = true)
    {
        float defaultValue;
        switch (name)
        {
            case Effect.Alchemist:
                Effects.Add(name);
                defaultValue = _magicResist;
                _magicResist = Mathf.Clamp(increase ? _magicResist + value : _magicResist - value, 0, 10000);
			   
                yield return new WaitForSeconds(duration);
                Effects.Remove(name);
                _magicResist = (int)defaultValue;
                break;
        }
	    
    }
    
    public virtual void TakeDamage(int damage, DamageType type = 0, Effect fx = Effect.None)
    {
        if (!IsLocalPlayer) { return; }
        
        var dmg = damage - (type == DamageType.Magical ? _magicResist : _physicalResist);
	    
        if (damage > 0)
        {

            _currentHealth.Value -= dmg;
            _healthSlider.TakeDamage(dmg, type, fx);
        }
        else if(_currentHealth.Value > 0)
            _healthSlider.Defence(type);

        if (_currentHealth.Value <= 0)
            Die();
    }
    
    public virtual void Die()
    {
        Destroy(gameObject);
    }
}

