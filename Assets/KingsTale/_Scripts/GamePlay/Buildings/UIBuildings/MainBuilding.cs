using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshObstacle))]
public class MainBuilding : Building
{
    [SerializeField] private Transform _spawnpointTransform;
    [SerializeField] private ushort _workerUnitId;
    [SerializeField] private float _culDown;
    [SerializeField] private ResourcesStruct _resourcesToAdd;
    
    private IEnumerator LiveCycleRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_culDown);

            var request = new ServerAddResourcesRequestStruct
            {
                PlayerId = OwnerClientId,
                ResourcesToAdd = _resourcesToAdd
            };

            InputManager.Instance.HandleAddResourcesRequestRpc(request);
        }
    }

    [Rpc(SendTo.Owner)]
    public override void PlaceBuildingRpc()
    {
        _isPlaced.Value = true;
        StartCoroutine(LiveCycleRoutine());
    }
    
    public void BuyWorkerUnit()
    {
        if (!IsOwner) return;
        
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