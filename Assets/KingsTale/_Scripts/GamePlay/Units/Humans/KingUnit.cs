using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KingUnit : AttackUnit
{
    [SerializeField] private float _attackRange = 0.3f;
	[SerializeField] private float _animTime = 0.16f;
	[SerializeField] private AbilityCircle _abilCircle;
		
	private List<Ability> Abilities = new();
	private readonly int _attackInt = Animator.StringToHash("Attack");
	private EffectSystem _fxSys;
	private KingConfigSO _kingConf;
    
	protected override void Start()
	{
		base.Start();
		_fxSys = FindObjectOfType<EffectSystem>();
		_kingConf = _config as KingConfigSO;
		foreach (var ability in _kingConf.Abilities)
		{
			if(ability.Name == AbilityName.FireAttack)
				Abilities.Add(new FireAtkAbility(ability, _fxSys, this));
		}
	}
	public override void ApplyDamage(int factor = 1, DamageType type = 0, Effect fx = 0)
    {
		if (!_objectToAttack)
        {
			if(_needToStop)
				Complete();
			if(StackAttackObjects && StackAttackObjects.Group.Count != 0)
				SearchForEnemy();
			StopAttack();
        }
        else if (_objectToAttack.TryGetComponent(out Building building))
            building.TakeDamage(_damage * factor);
		else if (_objectToAttack.TryGetComponent(out EnemyUnit unit) ){
            unit.TakeDamage(_damage * factor);
			if(!unit._objectToAttack && !unit._dead)
				unit.StartAttack(gameObject);
		}
    }

	protected override void Update()
	{
		base.Update();
		if (_objectToAttack)
		{
			var pos = _objectToAttack.transform.position;
			transform.LookAt(new Vector3(pos.x, transform.position.y,pos.z));
		}
	}
	
    protected override IEnumerator Attack()
    {
        if (!_objectToAttack)
        {
			if(_needToStop)
			{
				Complete();
			}
			if(StackAttackObjects && StackAttackObjects.Group.Count != 0)
				SearchForEnemy();
			StopAttack();
            yield break;
        }
        if (Mathf.Abs(_agent.velocity.magnitude) <= 0.05 && Physics.Raycast(transform.position, transform.forward, out var hit, _attackRange))
        {
	        var hitCollider = hit.collider;
            if (!hit.collider.isTrigger && (hit.collider.gameObject.transform.parent && hit.collider.gameObject.transform.parent.gameObject == _objectToAttack) || hit.collider.gameObject == _objectToAttack)
            {
	            
                _animator.SetBool(_attackInt, true);
				yield return new WaitForSeconds(_animTime);
				if(!hitCollider){
					StartCoroutine(Attack());
                	yield break;
				}

	            ApplyDamage(hitCollider.gameObject.name == "UnitWeakPlace" ? 2:1);
	            _animator.SetBool(_attackInt, false);
                yield return new WaitForSeconds(_culDown - _animTime);
                _attackCoroutine = StartCoroutine(Attack());
                yield break;
            }
            
        }
        _animator.SetBool(_attackInt, false);
        yield return new WaitForSeconds(0.5f);
        _attackCoroutine = StartCoroutine(Attack());
    }

    public void UseAbility(int i)
    {
	    if (i >= Abilities.Count) return;
	    
	    _abilCircle.gameObject.SetActive(true);
	    _abilCircle.Script = Abilities[i];
	    _abilCircle.GetComponent<SphereCollider>().radius = _kingConf.Abilities[i].Radius;
	    Abilities[i].Use();
	    _abilCircle.Deactivate();
    }
}
