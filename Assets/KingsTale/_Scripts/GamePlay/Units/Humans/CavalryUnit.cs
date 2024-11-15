using System.Collections;
using UnityEngine;

public class CavarlyUnit : AttackUnit
{
	[SerializeField] private int _damageFactor = 2;
	[SerializeField] private float _retreatDist = 5f;

	private bool _attack;

	protected override void Start()
	{
		base.Start();
		_agent.speed = _agent.acceleration;
	}
	public override void StartAttack(GameObject objectToAtk)
	{
		_visionCircle.SetActive(false);
		GoToPoint(objectToAtk.transform.position);
		StartCoroutine(Attack());
	}
	
	public override void ApplyDamage(int factor = 1, DamageType type = 0, Effect fx = 0)
	{
		if (_objectToAttack)
		{
			if (Effects.Contains(Effect.FireRage))
			{
				type = DamageType.Magical;
				fx = Effect.Pyro;
			}
			if (_objectToAttack.TryGetComponent(out Building building))
				building.TakeDamage(_damage * factor, type, fx);
			if (!_objectToAttack.TryGetComponent(out EnemyUnit unit)) return;
			unit.TakeDamage(_damage * factor, type, fx);
			if(!unit._objectToAttack && !unit._dead)
				unit.StartAttack(gameObject);
		}
		else
		{
			if (_needToStop)
			{
				Complete();
			}
			_needToStop = false;
			_animator.SetBool("Attack", false);
			if (StackAttackObjects && StackAttackObjects.Group.Count != 0)
				SearchForEnemy();
		}
	}
	
	protected override void Update()
	{
		base.Update();
		
		if (!_attack || !Physics.Raycast(transform.position, transform.forward, out var hit, 1.3f)) return;
	    
	    if (hit.collider.gameObject.tag == "EnemyUnit")
	    {
		    _objectToAttack = hit.collider.transform.parent.gameObject;
		    if(_objectToAttack.transform.parent.TryGetComponent(out EnemyGroups group))
			    StackAttackObjects = group;

		    int factor = 1;
		    if (Mathf.Abs(_agent.velocity.magnitude) >= _config.Speed)
		    {
			    factor *= _damageFactor;
			    GoToPoint(transform.position + (transform.forward * (_retreatDist * 0.5f)));
		    }
		    else
		    {
			    Retreat();
		    }

		    if (hit.collider.gameObject.name == "UnitWeakPlace")
			    factor *= 2;
		    ApplyDamage(factor);
	    }
	}

	protected override IEnumerator Attack()
	{
		_attack = true;
		_agent.speed = _config.Speed;
		yield break;
	}

    private void Retreat()
    {
	    _agent.speed = _agent.acceleration;
	    GoToPoint(transform.position + (transform.forward * _retreatDist));

    }
	public override void Complete()
	{
		base.Complete();
		_attack = false;
		if (StackAttackObjects && StackAttackObjects.Group.Count != 0)
			SearchForEnemy();
		else if (_objectToAttack)
			StartAttack(_objectToAttack);
		else
			_visionCircle.SetActive(true);
	
	}
}
