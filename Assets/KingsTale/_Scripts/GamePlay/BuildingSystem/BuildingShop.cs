using Unity.Netcode;

public class BuildingShop : NetworkBehaviour
{
    public void BuyBuilding(int buildingId)
    {
        if (!IsLocalPlayer) return;
        
        var request = new ServerBuyRequestStruct();
        
        request.PlayerId = NetworkObject.OwnerClientId;
        request.Id = (ushort)buildingId;
        request.IsBuilding = true;
        
        InputManager.Instance.HandleBuyRequestRpc(request);
    }
}