using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

[SelectionBase]
[RequireComponent(typeof(NavMeshAgent))]
public abstract class UnitBrain : NetworkBehaviour, IDamagable
{
    [Header("Config")]
    [SerializeField] protected UnitBaseConfigSO _config;
    [SerializeField] private ushort _id;

    [Header("GameObjects")]
    [SerializeField] protected HealthSlider _healthSlider;

    private float _speed;
    private float _stopDictance;

    private NetworkVariable<int> _currentHealth = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    protected int _regeneration;
    protected float _regenSpeed;
	private int _magicResist;
	private int _physicalResist;
    private int _foodPrice;
    private int _goldPrice;
    private int _woodPrice;

    protected NavMeshAgent _agent;
    protected Animator _animator;
    
    public ushort Id => _id;
    
    public override void OnNetworkSpawn()
    {
	    if (!IsOwner) { return; }

	    _currentHealth.Value = (int)_config.MaxHealth;
	    _speed = _config.Speed;
        _stopDictance = _config.StopDistance;
        _regeneration = (int)_config.Regeneration;
        _regenSpeed = _config.RegenSpeed;
        _magicResist = (int)_config.MagicResist;
        _physicalResist = (int)_config.PhysicalResist;
        _id = _config.UnitId;

        _agent = GetComponent<NavMeshAgent>();
		_animator = transform.GetChild(1).GetComponent<Animator>();
		_agent.speed = _speed;
        _agent.stoppingDistance = _stopDictance * 0.5f;

        _healthSlider.SetMaxHealth(_currentHealth.Value);
    }
    protected virtual void Update()
    {
        if (_healthSlider)
        {
	        _healthSlider.transform.rotation = Quaternion.Euler(40,50,0);
	    }
    }

    protected IEnumerator InPath()
    {
	    while (_agent.pathPending || _agent.remainingDistance > _agent.stoppingDistance || _agent.velocity.sqrMagnitude > 0f)
	    {
		    yield return null;
	    }
    }
    
    protected void GoToPoint(Vector3 point)
    {
        _agent.SetDestination(point);
    }

    public virtual void TakeDamage(int damage)
    {
	    if (damage > 0)
		    _currentHealth.Value -= damage;

	    if (_currentHealth.Value <= 0)
	    {
		    var request = new ServerDieRequestStruct
		    {
				Id = NetworkObjectId,
				IsBuilding = false
		    };
		    
		    InputManager.Instance.HandleDieRequestRpc(request);
	    }
    }

    public virtual void Die()
	{
		
	}
    
    [Rpc(SendTo.Owner)]
    public virtual void SetDestinationRpc(Vector3 pos)
    {
	    GoToPoint(pos);
    }

    [Rpc(SendTo.Owner)]
    public virtual void SetBuildingRpc(ulong buildingID)
    {
	    var point = NetworkManager.Singleton.SpawnManager.SpawnedObjects[buildingID].transform.position;
	    GoToPoint(point);
    }
}