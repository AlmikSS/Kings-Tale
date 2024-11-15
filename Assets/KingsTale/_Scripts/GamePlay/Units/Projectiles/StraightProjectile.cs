using System.Collections;
using UnityEngine;

public class StraightProjectile : Projectile
{
    [SerializeField] private float _speed = 5f;
    private Vector3 _targetPos;
    
    public void FixedUpdate()
    {
        
        transform.position = Vector3.MoveTowards(transform.position, _targetPos, _speed * Time.fixedDeltaTime);
        if (transform.position == _targetPos)
        {
            Destroy(gameObject);
        }
    }

    public override void Launch()
    {
        _targetPos = new Vector3(TargetObject.position.x, transform.position.y, TargetObject.position.z);
        transform.LookAt(TargetObject);
        //transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y, 0);
        StartCoroutine(EnablePhysics());
    }


    public override void OnTriggerEnter(Collider other)
    {
        var type = DamageType.Physical;
        var fx = _effect.Name;
        if (other.gameObject.TryGetComponent(out Building building))
        {
            Brain.SetAttackObject(other.gameObject);
            _effect.TargetBulding = building;
            if (_effect.Name == Effect.Pyro)
            {
                _fxSys.ApplyEffect(_fxSys.Burn, _effect);
                type = DamageType.Magical;
            }
            Brain.ApplyDamage(1, type, fx);

            Destroy(gameObject);
        }////////////////ADD SERVER UNIT CHECK
        else if (other.transform.parent && other.transform.parent.gameObject.TryGetComponent(out UnitBrain unit)){
            Brain.SetAttackObject(other.transform.parent.gameObject);
            _effect.Target = unit;
            if (_effect.Name == Effect.Pyro)
            {
                _fxSys.ApplyEffect(_fxSys.Burn, _effect);
                type = DamageType.Magical;
            }

            Brain.ApplyDamage(other.gameObject.name != "UnitWeakPlace" ? 1 : 2, type, fx);

            Destroy(gameObject);
        }
        else if(other.gameObject.tag == "Ground"){
            Destroy(gameObject);
        }
    }

	private IEnumerator EnablePhysics(){
		yield return new WaitForSeconds(_autoDestroy);
		gameObject.GetComponent<Rigidbody>().useGravity = true;
	}
}
