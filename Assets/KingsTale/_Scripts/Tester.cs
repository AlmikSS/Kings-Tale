using Unity.Netcode;
using UnityEngine;

public class Tester : NetworkBehaviour
{
    private void Update()
    {
        if (!IsLocalPlayer) return;
        
        if (Input.GetKeyDown(KeyCode.A))
        {
            InputManager.Instance.HandleBuyRequestRpc(new ServerBuyRequestStruct(NetworkObject.OwnerClientId, 1, true));
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            var res = new ResourcesStruct(1, 12, 0);
            InputManager.Instance.HandleAddResourcesRequestRpc(new ServerAddResourcesRequestStruct(res, NetworkObject.OwnerClientId));
        }
    }
}