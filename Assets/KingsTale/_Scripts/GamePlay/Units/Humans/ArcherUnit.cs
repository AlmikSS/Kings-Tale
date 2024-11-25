using System.Collections;
using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public class ArcherUnit : AttackUnit
{
    [SerializeField] private ushort _projectileId;

    protected override IEnumerator AttackTargetRoutine()
    {
        // Включаем анимацию атаки
        SetAttackAnimationServerRpc(true);
        var request = new ServerSpawnProjectileRequestStruct
        {
            Id = NetworkObjectId,
            ProjectileId = _projectileId,
            Position = transform.position
        };

        InputManager.Instance.HandleSpawnProjectileRequestRpc(request);
        yield return null;
        // Выключаем анимацию атаки
        SetAttackAnimationServerRpc(false);
        yield return new WaitForSeconds(_attackSpeed);
    }

    [Rpc(SendTo.Owner)]
    public void OnProjectTileSpawnedRpc(NetworkObjectReference networkObjectReference)
    {
        if (networkObjectReference.TryGet(out NetworkObject networkObject))
        {
            if (networkObject.TryGetComponent(out Projectile projectile))
                projectile.Launch(_target, _damage);
        }
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
