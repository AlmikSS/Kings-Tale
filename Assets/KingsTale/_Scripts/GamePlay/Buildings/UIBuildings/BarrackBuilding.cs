using UnityEngine;

public class BarrackBuilding : Building
{
    [SerializeField] private Transform _spawnpointTransform;

    public void BuyUnit(int id)
    {
        if (!IsLocalPlayer) { return; }

        if (!_isBuilt.Value) { return; }
        
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