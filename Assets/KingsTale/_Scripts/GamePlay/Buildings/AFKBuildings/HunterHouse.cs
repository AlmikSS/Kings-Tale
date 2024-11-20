using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class HunterHouse : AFKBuilding
{
    protected override IEnumerator LiveCycleRoutine()
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
}