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

    private const float ROTATION_X = 40f;
    private const float ROTATION_Y = -25f;
    private const float ROTATION_Z = 0f;
    private const float STOP_DISTANCE_MULTIPLIER = 0.5f;
    private const float DIE_DELAY = 2f;

    public int Health => _currentHealth.Value;
    public bool IsDied { get; private set; }
    public ushort Id => _id;
    
    private float _speed;
    private float _stopDistance;
    private int _magicResist;
    private int _physicalResist;
    private int _foodPrice;
    private int _goldPrice;
    private int _woodPrice;

    private NetworkVariable<int> _currentHealth;
    protected int _regeneration;
    protected float _regenSpeed;

    protected NavMeshAgent _agent;
    protected Animator _animator;

    protected virtual void Awake()
    {
        _currentHealth = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);        
    }
    
    public override void OnNetworkSpawn()
    {
        if (!IsOwner)
        {
            enabled = false;
            return;
        }

        InitializeUnit();
        SetupComponents();
    }

    protected virtual void Update()
    {
        UpdateHealthSliderRotation();
        //UpdateAnimatorVelocity();
    }

    protected IEnumerator InPath()
    {
        while (!IsPathComplete())
        {
            yield return null;
        }
    }
    
    protected void GoToPoint(Vector3 point)
    {
        if (IsDied) return;

        if (!_agent.isOnNavMesh)
        {
            Debug.LogWarning($"Unit {NetworkObjectId} is not on NavMesh!");
            return;
        }
        
        _agent.SetDestination(point);
    }

    [Rpc(SendTo.Owner)]
    public virtual void TakeDamageRpc(int damage)
    {
        if (damage <= 0) return;
        
        _currentHealth.Value -= damage;
        _healthSlider?.TakeDamage(damage);

        if (_currentHealth.Value <= 0)
        {
            Die();
        }
    }    

    public virtual void Die()
    {
        if (IsDied) return;
        
        IsDied = true;
        CleanupOnDeath();
        Invoke(nameof(DieRpc), DIE_DELAY);
        //_animator?.SetBool(GamePlayConstants.DIE_ANIMATOR_PAR, true);
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
    public virtual void SetBuildingRpc(ulong buildingId)
    {
        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(buildingId, out var spawnedObject))
        {
            Debug.LogError($"Building with ID {buildingId} not found!");
            return;
        }

        GoToPoint(spawnedObject.transform.position);
    }

    private void InitializeUnit()
    {
        if (_config == null)
        {
            Debug.LogError($"Unit {NetworkObjectId} config is missing!");
            return;
        }

        _currentHealth.Value = (int)_config.MaxHealth;
        _speed = _config.Speed;
        _stopDistance = _config.StopDistance;
        _regeneration = (int)_config.Regeneration;
        _regenSpeed = _config.RegenSpeed;
        _magicResist = (int)_config.MagicResist;
        _physicalResist = (int)_config.PhysicalResist;
        _id = _config.UnitId;
    }

    private void SetupComponents()
    {
        _agent = GetComponent<NavMeshAgent>();
        if (_agent == null)
        {
            Debug.LogError($"NavMeshAgent component missing on unit {NetworkObjectId}!");
            return;
        }

        Transform modelTransform = transform.GetChild(1);
        if (modelTransform != null)
        {
            _animator = modelTransform.GetComponent<Animator>();
        }

        _agent.speed = _speed;
        _agent.stoppingDistance = _stopDistance * STOP_DISTANCE_MULTIPLIER;

        if (_healthSlider != null)
        {
            _healthSlider.SetMaxHealth(_currentHealth.Value);
        }
    }

    private void UpdateHealthSliderRotation()
    {
        if (_healthSlider != null)
        {
            _healthSlider.transform.rotation = Quaternion.Euler(ROTATION_X, ROTATION_Y, ROTATION_Z);
        }
    }

    private void CleanupOnDeath()
    {
        if (_agent != null)
        {
            _agent.ResetPath();
        }
        StopAllCoroutines();
    }

    protected bool IsPathComplete()
    {
        return !_agent.pathPending && 
               _agent.remainingDistance <= _agent.stoppingDistance && 
               _agent.velocity.sqrMagnitude == 0f;
    }

    /*private void UpdateAnimatorVelocity()
    {
        if (_animator != null)
        {
            _animator.SetFloat(GamePlayConstants.VELOCITY_ANIMATOR_PAR, _agent.velocity.magnitude);
        }
    }*/
}