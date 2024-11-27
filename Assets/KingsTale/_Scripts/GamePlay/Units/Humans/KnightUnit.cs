using System.Collections;
using Unity.Netcode.Components;
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

        // Включаем анимацию атаки
        SetAttackAnimationServerRpc(true);
        InputManager.Instance.HandleTakeDamageRequestRpc(request);
        yield return null;
        // Выключаем анимацию атаки
        SetAttackAnimationServerRpc(false);
        yield return new WaitForSeconds(_attackSpeed);
    }

    // RPC для управления анимацией Attack
    [ServerRpc(RequireOwnership = false)]
    private void SetAttackAnimationServerRpc(bool isAttacking)
    {
        SetAttackAnimationClientRpc(isAttacking);
    }

    [ClientRpc]
    private void SetAttackAnimationClientRpc(bool isAttacking)
    {
        if (_networkAnimator == null || _networkAnimator.Animator == null)
        {
            Debug.LogError("_networkAnimator or Animator is null on " + gameObject.name);
            return;
        }
        _networkAnimator.Animator.SetBool(GamePlayConstants.ATTACK_ANIMATOR_PAR, isAttacking);
    }

}
