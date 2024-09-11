using System.Threading.Tasks;
using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    public void Initialize()
    {
        if (!IsServer)
            Destroy(gameObject);
    }

    public void HandleBuyRequest(ServerBuyRequestStruct request)
    {
        
    }
    
    public async Task<bool> IsPlayerExist(ushort id)
    {
        return true;
    }

    public async Task<bool> IsBuildingIdExist(ushort id)
    {
        return true;
    }

    public async Task<bool> IsUnitIdExist(ushort id)
    {
        return true;
    }
}