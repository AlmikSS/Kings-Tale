using System.Collections;
using UnityEngine;

public class KnightUnit : AttackUnit
{
    protected override IEnumerator AttackTargetRoutine()
    {
        var request = new ServerTakeDamageRequestStruct
        {
            PlayerId = OwnerClientId,
            Damage = _damage,
            Id = _target.NetworkObjectId
        };
        
        InputManager.Instance.HandleTakeDamageRequestRpc(request);
        yield return new WaitForSeconds(_attackSpeed);
    }
}