using Unity.Netcode;
using UnityEngine;

public class HouseBuilding : Building
{
    [Rpc(SendTo.Owner)]
    public override void BuildRpc()
    {
        if (!IsOwner) { return; }
        
        _isBuilt.Value = true;
        GetComponentInChildren<MeshRenderer>().sharedMaterial.color = Color.white;

        var request = new ServerAddUnitsPlaceRequestStruct
        {
            PlayerId = OwnerClientId
        };
        
        InputManager.Instance.HandleAddUnitsPlaceRpc(request);
    }
}