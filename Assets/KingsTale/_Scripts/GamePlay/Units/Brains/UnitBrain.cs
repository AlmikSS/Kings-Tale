using System.Collections;
using Unity.Netcode;
using Unity.Netcode.Components;
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

    public int Health => _currentHealth.Value;
    public bool IsDied { get; private set; }
    
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

    private NetworkVariable<float> _networkVelocity = new NetworkVariable<float>(
        0f,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Owner);

    protected NavMeshAgent _agent;
    protected Animator _animator;
    protected NetworkAnimator _networkAnimator;

    public ushort Id => _id;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        // Initialize components for all clients
        _animator = GetComponentInChildren<Animator>();
        if (_animator == null)
        {
            Debug.LogError("Animator not found on " + gameObject.name);
        }

        _agent = GetComponent<NavMeshAgent>();
        if (_agent == null)
        {
            Debug.LogError("NavMeshAgent not found on " + gameObject.name);
        }

        _networkAnimator = GetComponentInChildren<NetworkAnimator>();
        if (_networkAnimator == null)
        {
            Debug.LogError("NetworkAnimator not found on " + gameObject.name);
        }
        else if (_networkAnimator.Animator == null)
        {
            Debug.LogError("Animator not assigned in NetworkAnimator on " + gameObject.name);
        }

        // Owner-specific initialization
        if (IsOwner)
        {
            // Existing initialization code for owner
            _currentHealth.Value = (int)_config.MaxHealth;
            _speed = _config.Speed;
            _stopDictance = _config.StopDistance;
            _regeneration = (int)_config.Regeneration;
            _regenSpeed = _config.RegenSpeed;
            _magicResist = (int)_config.MagicResist;
            _physicalResist = (int)_config.PhysicalResist;
            _id = _config.UnitId;

            int velocityParamHash = Animator.StringToHash(GamePlayConstants.VELOCITY_ANIMATOR_PAR);

            _agent.speed = _speed;
            _agent.stoppingDistance = _stopDictance * 0.5f;

            _healthSlider.SetMaxHealth(_currentHealth.Value);
        }
    }

    protected virtual void Update()
    {
        if (_healthSlider)
        {
            _healthSlider.transform.rotation = Quaternion.Euler(40, -25, 0);
        }

        if (_agent == null)
        {
            Debug.LogError("_agent is null on " + gameObject.name);
            return;
        }

        if (_networkAnimator == null || _networkAnimator.Animator == null)
        {
            Debug.LogError("_networkAnimator or Animator is null on " + gameObject.name);
            return;
        }


        if (IsOwner)
        {
            _networkVelocity.Value = _agent.velocity.magnitude;
        }

        _networkAnimator.Animator.SetFloat(GamePlayConstants.VELOCITY_ANIMATOR_PAR, _networkVelocity.Value);

        // Determine if the unit is moving
        bool isMoving = _agent.velocity.magnitude > 0.1f;

        // Set the IsRunning parameter
        _networkAnimator.Animator.SetBool(GamePlayConstants.RUN_ANIMATOR_PAR, isMoving);
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
	    if (IsDied) return;
	    
        _agent.SetDestination(point);
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
        if (IsDied) return;

        if (_networkAnimator != null && _networkAnimator.Animator != null)
        {
            _networkAnimator.Animator.SetBool(GamePlayConstants.DIE_ANIMATOR_PAR, true);
        }
        else
        {
            Debug.LogError("Cannot play death animation, Animator is null on " + gameObject.name);
        }

        IsDied = true;
        StopAllCoroutines();
        Invoke(nameof(DieRpc), 2f);
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

    [ClientRpc]
    private void UpdateVelocityClientRpc(float velocity)
    {
        _networkAnimator.Animator.SetFloat(GamePlayConstants.VELOCITY_ANIMATOR_PAR, velocity);
    }
}