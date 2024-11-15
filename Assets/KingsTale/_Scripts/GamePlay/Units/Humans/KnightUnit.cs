using System.Collections;
using UnityEngine;

public class KnightUnit : AttackUnit
{
	[SerializeField] private float _attackRange = 0.3f;
	[SerializeField] private float _animTime = 0.16f;
    public override void ApplyDamage(int factor = 1, DamageType type = 0, Effect fx = 0)
    {
	    if (_objectToAttack)
	    {
		    if (Effects.Contains(Effect.FireRage) && type == DamageType.Physical)
		    {
			    type = DamageType.Magical;
			    fx = Effect.Pyro;
		    }
		    if (_objectToAttack.TryGetComponent(out Building building))
			    building.TakeDamage(_damage * factor, type, fx);
		    if (!_objectToAttack.TryGetComponent(out UnitBrain unit)) return;
		    unit.TakeDamage(_damage * factor, type, fx);

	    }
	    else if (!_objectToAttack)
	    {
		    if(_needToStop) 
			    Complete();
		    StopAttack();
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
        RaycastHit hit;
        if (!_objectToAttack)
        {
			if(_needToStop)
			{
				Complete();
			}
			StopAttack();
            yield break;
        }
        if (Mathf.Abs(_agent.velocity.magnitude) <= 0.05 && Physics.Raycast(transform.position, transform.forward, out hit, _attackRange))
        {
	        ////////////////ADD SERVER UNIT CHECK
            if (!hit.collider.isTrigger && (hit.collider.gameObject.transform.parent && hit.collider.gameObject.transform.parent.gameObject == _objectToAttack) || hit.collider.gameObject == _objectToAttack)
            {
                
                _animator.SetBool("Attack", true);
				yield return new WaitForSeconds(_animTime);
				if(!hit.collider){
					StartCoroutine(Attack());
                	yield break;
				}

	            ApplyDamage(hit.collider.gameObject.name == "UnitWeakPlace" ? 2:1);
	            _animator.SetBool("Attack", false);
                yield return new WaitForSeconds(_culDown - _animTime);
                _attackCoroutine = StartCoroutine(Attack());
                yield break;
            }
            
        }
        _animator.SetBool("Attack", false);
        yield return new WaitForSeconds(0.5f);
        _attackCoroutine = StartCoroutine(Attack());
    }
}