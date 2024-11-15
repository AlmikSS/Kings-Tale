using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Netcode;

public abstract class AttackUnit : UnitBrain
{
    
    protected float _culDown;
    protected int _damage;
	protected GameObject _visionCircle;
    [HideInInspector] public EnemyGroups StackAttackObjects;
    [HideInInspector] public GameObject _objectToAttack;
    protected float _stopUnitDictance;
    private Coroutine _checkDestiny;
    public UnitAttackConfigSO AttackConfig { get; private set; }
    protected Coroutine _attackCoroutine;
    
    protected override void Start()
    {
        base.Start();
        AttackConfig = (UnitAttackConfigSO)_config;
        
        _culDown = AttackConfig.AttackSpeed;
        _damage = (int)AttackConfig.Damage;
        _stopUnitDictance = _stopDictance * 0.25f;
		if(transform.Find("EnemyVision"))
			_visionCircle = transform.Find("EnemyVision").gameObject;
    }
    
    public override void Initialize()
    {
        //PlayerMng.PlayerUnits.WarUnits.Add(this);
        base.Initialize();
    }

    protected override void Update()
    {
        base.Update();
        if (_objectToAttack && Vector3.Distance(transform.position, _targetPos) <= _stopUnitDictance)
        {
            Complete();
        }
        else if (!_objectToAttack && Vector3.Distance(transform.position,_targetPos)<=_stopDictance)
        {
            if (StackOfPositions is { Count: > 0 })
            {
                if(_checkDestiny != null)
                    StopCoroutine(_checkDestiny);
                GoToPoint(StackOfPositions[0]);
                StackOfPositions.RemoveAt(0);
                _checkDestiny = StartCoroutine(CheckDestiny());
            }
            else{
                Complete();
            }
        }
    }

    public abstract void ApplyDamage(int factor = 1, DamageType type = 0, Effect fx = 0);
    
    public virtual void StartAttack(GameObject objectToAtk)
    {
        if(_attackCoroutine != null)
            StopCoroutine(_attackCoroutine);
        _objectToAttack = objectToAtk;
        
        Physics.Raycast(transform.position, new Vector3(_objectToAttack.transform.localPosition.x-transform.position.x,transform.position.y,_objectToAttack.transform.localPosition.z-transform.position.z), out var hit, Mathf.Infinity, 1 << 11);
        GoToPoint(hit.transform && hit.transform.name == objectToAtk.name ? hit.point : objectToAtk.transform.position);

        _attackCoroutine = StartCoroutine(Attack());
    }
    
    protected abstract IEnumerator Attack();
    
    public void SearchForEnemy(bool isAttack = true)
    {
        List<float> distances = new List<float>();
        foreach(var enemy in StackAttackObjects.Group)
        {
            distances.Add(Vector3.Distance(transform.position, enemy.transform.position));
        }
		if(isAttack)
        	StartAttack(StackAttackObjects.Group[distances.IndexOf(distances.Min())]);
		else
			GoToPoint(StackAttackObjects.Group[distances.IndexOf(distances.Min())].transform.position);
    }
    
    public virtual void StopAttack()
    {
        _objectToAttack = null;
		_needToStop = false;
        _animator.SetBool("Attack", false);
        if(_attackCoroutine != null)
            StopCoroutine(_attackCoroutine);
    }
    
    protected override IEnumerator Regeneration()
    {
        yield return new WaitForSeconds(5f);
        while(_currentHealth < _maxHealth && !_objectToAttack)
        {
            _currentHealth += _regeneration;
            _healthSlider.Heal(_regeneration);
            yield return new WaitForSeconds(_regenSpeed);
        }
    }
    
    public override void Die()
    {
        _objectToAttack = null;
        StackAttackObjects = null;
        _animator.SetBool("Attack", false);
        if(this is not EnemyUnit)
            //PlayerMng.PlayerUnits.Deselct(this);
        //PlayerMng.PlayerUnits.Units.Remove(this);
        //PlayerMng.PlayerUnits.WarUnits.Remove(this);
        foreach (var btn in FindObjectsOfType<UnitsGroupButton>())
        {
            btn.Group.Remove(this);
            btn.UpdateText();
        }
        base.Die();
    }
    public void SetAttackObject(GameObject obj) => _objectToAttack = obj;
    public void SetBuildingToVision(Building obj) => _visionCircle.GetComponent<VisionCircle>().BuildingToAttack = obj;

    private IEnumerator CheckDestiny()
    {
        yield return new WaitForSeconds(4f);
        if (StackOfPositions is { Count: > 0 })
        {
            GoToPoint(StackOfPositions[0]);
            StackOfPositions.RemoveAt(0);
            _checkDestiny = StartCoroutine(CheckDestiny());
        }
        else{
            Complete();
        }
    }
    protected virtual void OnDestroy(){}
    
    public override void SetBuildingRpc(ulong buildingID)
    {
        var building = NetworkManager.Singleton.SpawnManager.SpawnedObjects[buildingID].GetComponent<GameObject>();
        SetAttackObject(building);
    }
}
