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
    }

    [Rpc(SendTo.Owner)]
    public void TestRpc()
    {
        Debug.Log("Handle TestRpc");
    }
}