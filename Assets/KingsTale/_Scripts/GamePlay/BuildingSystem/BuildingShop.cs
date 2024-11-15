using Unity.Netcode;
using UnityEngine;

public class BuildingShop : MonoBehaviour
{
    [SerializeField] private GameplayButtons _gameplayButtons;
    
    public void BuyBuilding(int buildingId)
    {
        var request = new ServerBuyRequestStruct
        {
            PlayerId = _gameplayButtons.PlayerId,
            Id = (ushort)buildingId,
            IsBuilding = true
        };

        InputManager.Instance.HandleBuyRequestRpc(request);
    }
}