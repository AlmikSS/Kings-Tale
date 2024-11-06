using Unity.Netcode;

public class BuildingShop : NetworkBehaviour
{
    public void BuyBuilding(ushort buildingId)
    {
        if (!IsLocalPlayer) return;
        
        var request = new ServerBuyRequestStruct();
        
        request.PlayerId = NetworkObject.OwnerClientId;
        request.Id = buildingId;
        request.IsBuilding = true;
        
        InputManager.Instance.HandleBuyRequestRpc(request);
    }

    [Rpc(SendTo.Owner)]
    public void HandleBuyBuildingResponseRpc(ClientBuyResponseStruct response)
    {
        if (!IsLocalPlayer) return;
        
        if (response.CanBuy && response.PlayerId == NetworkObject.OwnerClientId)
        {
            //TODO Building system - start building
        }
    }
}