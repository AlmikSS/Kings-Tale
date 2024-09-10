using UnityEngine;

public class ServerLogicTester : GameEntity
{
    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            NetworkObject.Despawn();
            Destroy(gameObject);
        }
    }
    
    public override void UpdateTick()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            InputManager.Instance.HandleBuyBuildingRequest(new BuyBuildingRequestStruct(0, 0));
    }
}