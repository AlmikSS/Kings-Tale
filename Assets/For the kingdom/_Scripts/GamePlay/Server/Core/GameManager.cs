using Unity.Netcode;

public class GameManager : NetworkBehaviour
{
    private GameData _gameData;
    private PlayersData _playersData;
    
    public void Initialize(GameData gameData, PlayersData playersData)
    {
        if (!IsServer)
            Destroy(gameObject);

        _gameData = gameData;
        _playersData = playersData;
    }

    public void HandleBuyRequest(ServerBuyRequestStruct request)
    {
        
    }
    
    public bool IsPlayerExist(ushort id)
    {
        var player = _playersData.GetPlayer(id);
        return player != null;
    }

    public bool IsBuildingIdExist(ushort id)
    {
        return true;
    }

    public bool IsUnitIdExist(ushort id)
    {
        return true;
    }

    private bool CanPlayerBuy(ServerBuyRequestStruct request)
    {
        
        return true;
    }
}