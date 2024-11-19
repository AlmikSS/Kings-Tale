using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class HunterHouse : Building
{
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
        if (!IsOwner) { return; }
        
        _isPlaced.Value = true;
    }
    
    [Rpc(SendTo.Owner)]
    public override void BuildRpc()
    {
        if (!IsOwner) { return; }

        _isBuilt.Value = true;
        GetComponentInChildren<MeshRenderer>().sharedMaterial.color = Color.white;

        StartCoroutine(LiveCycleRoutine());
    }
}