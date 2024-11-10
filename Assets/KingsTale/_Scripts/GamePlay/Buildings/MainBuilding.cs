using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshObstacle))]
public class MainBuilding : Building
{
    [SerializeField] private Transform _spawnpointTransform;
    [SerializeField] private ushort _workerUnitId;
    
    public void BuyWorkerUnit()
    {
        if (!IsLocalPlayer) return;
        
        var request = new ServerBuyRequestStruct
        {
            IsBuilding = false,
            PlayerId = NetworkObject.OwnerClientId,
            Position = _spawnpointTransform.position,
            Id = _workerUnitId
        };
        InputManager.Instance.HandleBuyRequestRpc(request);
    }
}