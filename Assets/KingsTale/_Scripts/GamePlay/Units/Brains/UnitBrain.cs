using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[SelectionBase]
[RequireComponent(typeof(NavMeshAgent))]
public abstract class UnitBrain : MonoBehaviour, IDamagable
{
    [Header("Config")]
    [SerializeField] protected UnitBaseConfigSO _config;

    [Header("GameObjects")]
    [SerializeField] protected HealthSlider _healthSlider;
    
    protected float _speed;
    protected float _stopDictance;

    protected int _maxHealth;
    protected int _currentHealth;
    protected int _regeneration;
    protected float _regenSpeed;
	private int _magicResist;
	private int _physicalResist;
    private int _foodPrice;
    private int _goldPrice;
    private int _woodPrice;
    protected internal bool _dead;

    protected Animator _animator;
    protected NavMeshAgent _agent;

    protected PlayerManager _player;
    
    protected Vector3 _targetPos;
    private bool _complited;

    private Coroutine _regenCoroutine;
    //Properties
    public PlayerManager PlayerMng => _player;
    public UnitBaseConfigSO Config => _config;
    public int FoodPrice => _foodPrice;
    public int GoldPrice => _goldPrice;
    public int WoodPrice => _woodPrice;
	
	protected bool _needToStop = true;
	
    [HideInInspector] public List<Vector3> StackOfPositions = new();

    [HideInInspector] public List<Effect> Effects = new();
    
    protected virtual void Start()
    {
        _speed = _config.Speed;
        _stopDictance = _config.StopDistance;
        _maxHealth = (int)_config.MaxHealth;
        _regeneration = (int)_config.Regeneration;
        _regenSpeed = _config.RegenSpeed;
        _magicResist = (int)_config.MagicResist;
        _physicalResist = (int)_config.PhysicalResist;

 
        _currentHealth = _maxHealth;
		_agent = GetComponent<NavMeshAgent>();
		_animator = transform.GetChild(1).GetComponent<Animator>();
		_agent.speed = _speed;
        _agent.stoppingDistance = _stopDictance * 0.5f;

        _healthSlider.SetMaxHealth(_maxHealth);
    }
    protected virtual void Update()
    {

        if (_healthSlider)
        {
	        _healthSlider.transform.rotation = Quaternion.Euler(40,50,0);
	    }
        
    }
    
    protected IEnumerator UpdateAnimations()
    {
		yield return new WaitForSeconds(0.1f);
        _animator.SetFloat("Velocity", Mathf.Abs(_agent.velocity.magnitude));
        StartCoroutine(UpdateAnimations());
    }
    
    public void GoToPoint(Vector3 point)
    {
        _agent.SetDestination(point);
        _complited = false;
        _targetPos = point;
    }
    
	public virtual void Walk(){}

    public bool IsComplited()
    {
        return _complited;
    }
    
    public virtual void Complete()
    {
        _agent.SetDestination(transform.position);
		StackOfPositions = new();
        _targetPos = transform.position;
        _complited = true;
		_needToStop = true;
    }

    public IEnumerator ChangeParam(Effect name, int value, float duration, bool increase = true)
    {
	    float defaultValue;
	    switch (name)
	    {
		   case Effect.Heal:
			   _regeneration = increase ? _regeneration * value : _regeneration / value;
			   _regenSpeed = increase ? _regenSpeed * value : _regenSpeed / value;
			   if(increase)
				   Effects.Add(name);
			   else
				   Effects.Remove(name);
			   break;
		   case Effect.FireShield:
			   if (Effects.Contains(name))
			   {
				   Effects.Remove(name);
				   _magicResist = Mathf.Clamp(_magicResist - value/100 * _magicResist, 0, 10000);
				   _physicalResist = Mathf.Clamp(_physicalResist - value/100 * _physicalResist, 0, 10000);
			   }
			   else
			   {
				   Effects.Add(name);
				   _magicResist = Mathf.Clamp(_magicResist + value/100 * _magicResist, 0, 10000);
				   _physicalResist = Mathf.Clamp(_physicalResist + value/100 * _physicalResist, 0, 10000);
			   }

			   break;
		   case Effect.InstantHeal:
			   _currentHealth = Mathf.Clamp(_currentHealth + value, 0, _maxHealth);
			   break;
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

    protected virtual IEnumerator Regeneration()
    {
	    yield return new WaitForSeconds(5f);
	    while(_currentHealth < _maxHealth)
	    {
		    _currentHealth += _regeneration;
		    _healthSlider.Heal(_regeneration);
		    yield return new WaitForSeconds(_regenSpeed);
	    }
    }
    
    public virtual void TakeDamage(int damage, DamageType type = 0, Effect fx = Effect.None)
    {
	    var dmg = damage - (type == DamageType.Magical ? _magicResist : _physicalResist);
	    
        if (damage > 0)
        {
	        if(_regenCoroutine != null)
				StopCoroutine(_regenCoroutine);
            _currentHealth -= dmg;
	        _healthSlider.TakeDamage(dmg, type, fx);
            _regenCoroutine = StartCoroutine(Regeneration());
        }
        else if(_currentHealth > 0)
	        _healthSlider.Defence(type);

        if (_currentHealth <= 0)
            Die();
    }

    public virtual void Die()
	{
		Complete();
		_animator.SetBool("Death", true);
		_healthSlider.gameObject.SetActive(false);
		_dead = true;
		StartCoroutine(Dead());
	}

	protected IEnumerator Dead(){
		yield return new WaitForSeconds(1.5f);
		Destroy(gameObject);
        //GetComponent<NetworkObject>().Despawn(true);
	}

    public void SetPlayer(PlayerManager manager)
    {
        if (manager)
            _player = manager;
    }
    
    public virtual void Initialize()
    {
        StartCoroutine(UpdateAnimations());
        //PlayerMng.PlayerUnits.Units.Add(this);
    }
    
    [Rpc(SendTo.Owner)]
    public void SetDestinationRpc(Vector3 pos)
    {
	    // метод
    }

    [Rpc(SendTo.Owner)]
    public void SetBuilding(ulong buildingID)
    {
	    var building = NetworkManager.Singleton.SpawnManager.SpawnedObjects[buildingID].GetComponent<Building>();
	    // метод
    }

}