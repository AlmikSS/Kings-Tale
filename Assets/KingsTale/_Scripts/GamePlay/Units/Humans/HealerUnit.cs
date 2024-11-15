using System.Collections;
using UnityEngine;
public class HealerUnit : AttackUnit
{
	[SerializeField] private float _auraDistance = 3.4f;
	
    public override void StartAttack(GameObject objectToAtk)
    {
        _objectToAttack = objectToAtk;
        Physics.Raycast(transform.position, new Vector3(_objectToAttack.transform.localPosition.x-transform.position.x,transform.position.y,_objectToAttack.transform.localPosition.z-transform.position.z), out var hit, Mathf.Infinity, 1 << 11);
        GoToPoint(hit.transform && hit.transform.name == _objectToAttack.name ? hit.point : _objectToAttack.transform.position);
    }

    public override void ApplyDamage(int factor = 1, DamageType type = 0, Effect fx = 0){}

    protected override void Update()
    {
	    base.Update();
	    if (!_objectToAttack) return;
	    var pos = _objectToAttack.transform.position;
	    transform.LookAt(new Vector3(pos.x, transform.position.y,pos.z));

	    if (Vector3.Distance(pos, transform.position) > _auraDistance)
	    {
		    Physics.Raycast(transform.position, new Vector3(_objectToAttack.transform.localPosition.x-transform.position.x,transform.position.y,_objectToAttack.transform.localPosition.z-transform.position.z), out var hit, Mathf.Infinity, 1 << 11);
		    GoToPoint(hit.transform && hit.transform.name == _objectToAttack.name ? hit.point : _objectToAttack.transform.position);
	    }
    }
    protected override IEnumerator Attack()
    {
	    yield break;
    }

}