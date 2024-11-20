public abstract class UIBuilding : Building
{
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        if (!IsOwner) { return; }
    }
}