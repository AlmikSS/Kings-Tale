using System.Collections;
using Unity.Netcode;
using UnityEngine;

public abstract class AFKBuilding : Building
{
    protected ResourcesStruct _resourcesToAdd;
    protected float _culDown;

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner) { return; }

        _resourcesToAdd = ((AFKBuildingConfigSO)_config).ResourcesToAdd;
        _culDown = ((AFKBuildingConfigSO)_config).CulDown;
    }
    
    protected abstract IEnumerator LiveCycleRoutine();
    
    [Rpc(SendTo.Owner)]
    public override void BuildRpc()
    {
        if (!IsOwner) { return; }

        _isBuilt.Value = true;
        GetComponentInChildren<MeshRenderer>().sharedMaterial.color = Color.white;

        StartCoroutine(LiveCycleRoutine());
    }
}