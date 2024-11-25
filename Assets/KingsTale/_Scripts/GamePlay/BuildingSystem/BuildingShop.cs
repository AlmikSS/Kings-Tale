using Unity.Netcode;
using UnityEngine;

public class BuildingShop : MonoBehaviour
{
    private PlayerManager _player;

    public void SetPlayer(PlayerManager player) => _player = player;
    
    public void BuyBuilding(int buildingId)
    {
        var request = new ServerBuyRequestStruct
        {
            PlayerId = _player.OwnerClientId,
            Id = (ushort)buildingId,
            IsBuilding = true
        };

        InputManager.Instance.HandleBuyRequestRpc(request);
    }
}