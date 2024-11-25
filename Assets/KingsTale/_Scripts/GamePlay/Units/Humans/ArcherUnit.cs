using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class ArcherUnit : AttackUnit
{
    [SerializeField] private ushort _projectileId;
    
    protected override IEnumerator AttackTargetRoutine()
    {
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