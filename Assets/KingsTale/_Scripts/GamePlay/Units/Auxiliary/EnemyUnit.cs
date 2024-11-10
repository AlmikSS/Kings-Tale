using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyUnit : AttackUnit
{
	[SerializeField] private PlayerManager _pm;
	[SerializeField] private float _animTime = 0.16f;
	private float _attackRange = 0.3f;
	
	private void Awake()
	{
		_player = _pm;
	}
    public override void ApplyDamage(int factor = 1, DamageType type = 0, Effect fx = 0)
    {
        if (!_objectToAttack)
        {
	        if(_needToStop)
	        {
		        Complete();
	        }
	        _needToStop = false;
	        _animator.SetBool("Attack", false);
	        StopCoroutine("Attack");
	        return;
        }
        if (_objectToAttack.TryGetComponent(out Building building))
	        building.TakeDamage(_damage * factor);
        if (_objectToAttack.TryGetComponent(out UnitBrain unit) )
	        unit.TakeDamage(_damage * factor);
        
    }
    public override void StartAttack(GameObject objectToAtk)
    {
	    _objectToAttack = objectToAtk;
	    GoToPoint(objectToAtk.transform.position);
	    StartCoroutine(Attack());
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
		    _needToStop = true;
		    _animator.SetBool("Attack", false);
		    StopCoroutine("Attack");
		    yield break;
	    }
	    if (Mathf.Abs(_agent.velocity.magnitude) <= 0.05 && Physics.Raycast(transform.position, transform.forward, out hit, _attackRange))
	    {
		    if (!hit.collider.isTrigger && (hit.collider.transform.parent && hit.collider.transform.parent.gameObject == _objectToAttack) || hit.collider.gameObject == _objectToAttack)
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
			    StartCoroutine(Attack());
			    yield break;
		    }
            
	    }
	    _animator.SetBool("Attack", false);
	    yield return new WaitForSeconds(0.5f);
	    StartCoroutine(Attack());
    }
	protected override void OnDestroy()
    {
		if(transform.parent.TryGetComponent(out EnemyGroups group))
            group.Group.Remove(gameObject);
    }
	public override void Initialize()
	{
        StartCoroutine(UpdateAnimations());
    }

}
