using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class ArcherUnit : AttackUnit
{
    [SerializeField] private ushort _projectileId;
    
    protected override IEnumerator AttackTargetRoutine()
    {
        //_animator.SetBool(GamePlayConstants.ATTACK_ANIMATOR_PAR, true);
        //yield return new WaitForSeconds(1.9f);
        //_animator.SetBool(GamePlayConstants.ATTACK_ANIMATOR_PAR, false);
        
        var request = new ServerSpawnProjectileRequestStruct
        {
            Id = NetworkObjectId,
            ProjectileId = _projectileId,
            Position = transform.position
        };
        
        InputManager.Instance.HandleSpawnProjectileRequestRpc(request);
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
}