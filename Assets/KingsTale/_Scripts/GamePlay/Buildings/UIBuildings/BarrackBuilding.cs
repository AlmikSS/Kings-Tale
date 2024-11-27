using UnityEngine;

public class BarrackBuilding : UIBuilding
{
    [SerializeField] private Transform _spawnpointTransform;

    public void BuyUnit(int id)
    {
        if (!_isBuilt.Value) { return; }

        if (!IsOwner) { return; }

        var request = new ServerBuyRequestStruct
        {
            IsBuilding = false,
            PlayerId = OwnerClientId,
            Id = (ushort)id,
            Position = _spawnpointTransform.position
        };
        
        InputManager.Instance.HandleBuyRequestRpc(request);
    }
}