using System.Collections;
using UnityEngine;

public class StormProjectile : Projectile
{
    [SerializeField] private Animator _animator;
    private bool _canDamage = true;
    public override void Launch() {}

    public void TurnOff()
    {
        _animator.SetBool("Active", false);
        Destroy(gameObject, 1f);
    }
    public override void OnTriggerEnter(Collider other){}
    public void OnTriggerStay(Collider other)
    {
        if (!_canDamage) return;
        
        
        const DamageType type = DamageType.Magical;
        var fx = _effect.Name;
        var lastObj = Brain._objectToAttack;
        if (other.gameObject.TryGetComponent(out Building building))
        {
            StartCoroutine(CanDamage());
            Brain.SetAttackObject(other.gameObject);
            _effect.TargetBulding = building;
            Brain.ApplyDamage(1, type, fx);
        }////////////////ADD SERVER UNIT CHECK
        else if (other.transform.parent && other.transform.parent.gameObject.TryGetComponent(out UnitBrain unit)){
            StartCoroutine(CanDamage());
            Brain.SetAttackObject(other.transform.parent.gameObject);
            _effect.Target = unit;
            if (_effect.Name == Effect.Freeze)
                _fxSys.ApplyEffect(_fxSys.Freeze, _effect);

            Brain.ApplyDamage(other.gameObject.name != "UnitWeakPlace" ? 1 : 2, type, fx);
        }
        if(lastObj)
            Brain.SetAttackObject(lastObj);
    }

    private IEnumerator CanDamage()
    {
        _canDamage = false;
        yield return new WaitForSeconds(1f);
        _canDamage = true;
    }
}
