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
        if (!IsLocalPlayer) { return; }
        
        _isPlaced.Value = true;
        StartCoroutine(LiveCycleRoutine());
    }
}