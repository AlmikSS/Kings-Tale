using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    private GameData _gameData;
    private PlayersData _playersData;
    
    public void Initialize(GameData gameData, PlayersData playersData)
    {
        _gameData = gameData;
        _playersData = playersData;
    }

    [Rpc(SendTo.Server)]
    public void HandleBuyRequestRpc(ServerBuyRequestStruct request)
    {
        Debug.Log($"Handle buy request. Client: {request.PlayerId}, Id: {request.Id}, IsBuilding: {request.IsBuilding}");
        var tester = NetworkManager.Singleton.ConnectedClients[request.PlayerId].PlayerObject.GetComponent<Tester>();
        if (tester != null)
            tester.TestRpc();
    }
    
    public bool IsPlayerExist(ulong id)
    {
        var player = _playersData.GetPlayer(id);
        //return player != null;
        return true;
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