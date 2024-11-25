using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class MainBuilding : UIBuilding
{
    [SerializeField] private Transform _spawnpointTransform;
    
    private ResourcesStruct _resourcesToAdd;
    private ushort _workerUnitId;
    private float _culDown;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner) { return; }

        _resourcesToAdd = ((MainBuildingConfigSO)_config).ResourcesToAdd;
        _workerUnitId = ((MainBuildingConfigSO)_config).WorkerUnitId;
        _culDown = ((MainBuildingConfigSO)_config).CulDown;
    }
    
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
        if (!IsOwner) { return; }
        
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