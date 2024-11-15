using Unity.Netcode;

public class BuildingShop : NetworkBehaviour
{
    public void BuyBuilding(int buildingId)
    {
        if (!IsLocalPlayer) return;
        
        var request = new ServerBuyRequestStruct
        {
            PlayerId = NetworkObject.OwnerClientId,
            Id = (ushort)buildingId,
            IsBuilding = true
        };

        InputManager.Instance.HandleBuyRequestRpc(request);
    }
}