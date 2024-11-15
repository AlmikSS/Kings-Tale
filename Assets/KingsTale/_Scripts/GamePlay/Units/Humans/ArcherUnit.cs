using System.Collections;
using UnityEngine;

public class ArcherUnit : AttackUnit
{
	[SerializeField] private GameObject _arrowPrefab;
	[SerializeField] private Transform _spawnPos;
    public override void StartAttack(GameObject objectToAtk)
    {
        _objectToAttack = objectToAtk;
        _attackCoroutine = StartCoroutine(Attack());
    }

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
	        StartCoroutine(NullObject());
			_needToStop = false;
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
	        StartCoroutine(NullObject());
            yield break;
        }

        _animator.SetBool("Attack", true);

		ArrowProjectile arrow = Instantiate(_arrowPrefab, _spawnPos.position, Quaternion.identity).GetComponent<ArrowProjectile>();
		arrow.TargetObject = _objectToAttack.transform;
		arrow.Brain = this;
		//arrow.gameObject.GetComponent<NetworkObject>().Spawn(true);
		arrow.Launch();
		yield return new WaitForSeconds(0.5f);
		_animator.SetBool("Attack", false);
		yield return new WaitForSeconds(_culDown - 0.5f);
		if (!_objectToAttack)
		{
			StartCoroutine(NullObject());
			yield break;
		}
		_attackCoroutine = StartCoroutine(Attack());
    }
	private IEnumerator NullObject(){
	    if(_needToStop){
		    Complete();
	    }
	    if(_attackCoroutine != null)
		    StopCoroutine(_attackCoroutine);
        yield return new WaitForSeconds(_culDown);
        _visionCircle.SetActive(false);
	}

	
}
