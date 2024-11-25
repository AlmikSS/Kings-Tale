using System.Collections;
using UnityEngine;

public class KnightUnit : AttackUnit
{
    protected override IEnumerator AttackTargetRoutine()
    {
        transform.LookAt(_target.transform.position);
        
        var request = new ServerTakeDamageRequestStruct
        {
            PlayerId = OwnerClientId,
            Damage = _damage,
            Id = _target.NetworkObjectId
        };

        _animator.SetBool(GamePlayConstants.ATTACK_ANIMATOR_PAR, true);
        InputManager.Instance.HandleTakeDamageRequestRpc(request);
        yield return null;
        _animator.SetBool(GamePlayConstants.ATTACK_ANIMATOR_PAR, false);
        yield return new WaitForSeconds(_attackSpeed);
    }
}