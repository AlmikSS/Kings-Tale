using System.Collections;
using UnityEngine;
public class WizardUnit : AttackUnit
{
    [SerializeField] private GameObject _projectile;
    [SerializeField] private Transform _spawnPos;
	[SerializeField] private float _animTime;
	private StormProjectile _storm;

    public override void StartAttack(GameObject objectToAtk)
    {
        _objectToAttack = objectToAtk;
        _attackCoroutine = StartCoroutine(Attack());
        StartCoroutine(DisableStorm());
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
		    if (_storm)
		    {
			    _storm.transform.LookAt(pos);
			    _storm.transform.rotation = Quaternion.Euler(90, _storm.transform.rotation.eulerAngles.y, _storm.transform.rotation.eulerAngles.z);
		    }
	    }
	    else if (_storm)
		    _storm.TurnOff();
    }
    
    protected override IEnumerator Attack()
    {
	    if (!_objectToAttack)
        {
	        StartCoroutine(NullObject());
            yield break;
        }

        _animator.SetBool("Attack", true);
		yield return new WaitForSeconds(_animTime);

		var projectile = Instantiate(_projectile, _spawnPos.position, Quaternion.Euler(90,0,0)).GetComponent<Projectile>();
		Vector3 pos =new Vector3();
		
		if (_objectToAttack)
			pos = _objectToAttack.transform.position;
		else
		{
			StartCoroutine(NullObject());
			yield break;
		}


		projectile.transform.LookAt(pos);
		projectile.transform.rotation = Quaternion.Euler(90, projectile.transform.rotation.eulerAngles.y, projectile.transform.rotation.eulerAngles.z);
		
		projectile.TargetObject = _objectToAttack.transform;
		projectile.Brain = this;
		projectile.Launch();
		if (projectile is StormProjectile)
		{
			if(_storm)
				_storm.TurnOff();
			_storm = projectile.GetComponent<StormProjectile>();
		}

		yield return new WaitForSeconds(0.05f);
		_animator.SetBool("Attack", false);
		yield return new WaitForSeconds(_culDown - 0.05f);

		if (!_objectToAttack)
		{
			StartCoroutine(NullObject());
			yield break;
		}

		if (!_storm)
			_attackCoroutine = StartCoroutine(Attack());
    }

    public override void StopAttack()
    {
	    base.StopAttack();
	    StartCoroutine(DisableStorm());
    }

    private IEnumerator DisableStorm()
    {
	    if (!_storm) yield break;
	    StopCoroutine(_attackCoroutine);
	    _storm.TurnOff();
	    yield return new WaitForSeconds(_culDown);
	    _attackCoroutine = StartCoroutine(Attack());
    }

	private IEnumerator NullObject(){
		if (_storm)
			_storm.TurnOff();
	    if(_needToStop){
		    Complete();
	    }
	    if(_attackCoroutine != null)
		    StopCoroutine(_attackCoroutine);
        yield return new WaitForSeconds(_culDown);
	}
}