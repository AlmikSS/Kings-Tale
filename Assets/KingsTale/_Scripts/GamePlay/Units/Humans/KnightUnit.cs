using System.Collections;
using Unity.Netcode;
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

        SetAttackAnimRpc();
        InputManager.Instance.HandleTakeDamageRequestRpc(request);
        yield return null;
        yield return new WaitForSeconds(_attackSpeed);
    }

    [Rpc(SendTo.Server)]
    private void SetAttackAnimRpc()
    {
        _networkAnimator.Animator.Play(GamePlayConstants.ATTACK_ANIMATOR_PAR);
    }
}
