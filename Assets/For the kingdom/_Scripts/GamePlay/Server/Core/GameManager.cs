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
        Debug.Log($"Handle buy request. Client: {request.PlayerId}, Id: {request.Id}, IsBuilding: {request.IsBuilding}.");

        var resources = _playersData.GetPlayerResources(request.PlayerId);
        var price = _gameData.GetPrice(request.Id, request.IsBuilding);
        var player = _playersData.GetPlayer(request.PlayerId);

        if (resources == null || price == null || player == null) return;

        if (CanPlayerBuy(resources, price))
        {
            player = _playersData.GetPlayer(request.PlayerId);
            
            if (request.IsBuilding)
            {
                //TODO Start placing building
            }
            else
            {
                _playersData.RemoveResourcesToPlayer(request.PlayerId, (ResourcesStruct)price);
                var unit = Instantiate(_gameData.GetUnit(request.Id), request.Position, Quaternion.identity);
                unit.GetComponent<NetworkObject>().SpawnWithOwnership(request.PlayerId);
            }
        }
    }

    [Rpc(SendTo.Server)]
    public void HandlePlaceBuildingRequestRpc(ServerPlaceBuildingRequestStruct request)
    {
        Debug.Log($"Handle place building request. Client: {request.PlayerId}, Building: {request.BuildingId}, Position: {request.Position}.");
        
        var resources = _playersData.GetPlayerResources(request.PlayerId);
        var price = _gameData.GetPrice(request.BuildingId, true);
        var player = _playersData.GetPlayer(request.PlayerId);

        if (resources == null || price == null || player == null) return;

        if (CanPlayerBuy(resources, price) && CanPlaceBuilding(request))
        {
            _playersData.RemoveResourcesToPlayer(request.PlayerId, (ResourcesStruct)price);
            var building = Instantiate(_gameData.GetBuilding(request.BuildingId), request.Position, Quaternion.identity);
            building.GetComponent<NetworkObject>().SpawnWithOwnership(request.PlayerId);
        }
    }

    [Rpc(SendTo.Server)]
    public void HandleAddResourcesRequestRpc(ServerAddResourcesRequestStruct request)
    {
        Debug.Log($"Handle add resources request. Client: {request.PlayerId}, Wood: {request.ResourcesToAdd.Wood}, Gold: {request.ResourcesToAdd.Gold}, Food: {request.ResourcesToAdd.Food}.");
        var player = _playersData.GetPlayer(request.PlayerId);

        if (player == null) return;
        
        _playersData.AddResourcesToPlayer(request.PlayerId, request.ResourcesToAdd);
    }
    
    public bool IsPlayerExist(ulong id)
    {
        var player = _playersData.GetPlayer(id);
        return player != null;
    }

    public bool IsBuildingIdExist(ushort id)
    {
        var building = _gameData.GetBuilding(id);
        return building != null;
    }

    public bool IsUnitIdExist(ushort id)
    {
        var unit = _gameData.GetUnit(id);
        return unit != null;
    }

    private bool CanPlayerBuy(ResourcesStruct? resources, ResourcesStruct? price)
    {
        return resources?.Wood >= price?.Wood && resources?.Gold >= price?.Gold && resources?.Food >= price?.Food;
    }

    private bool CanPlaceBuilding(ServerPlaceBuildingRequestStruct request)
    {
        var buildingPrefab = _gameData.GetBuilding(request.BuildingId);
        var building = Instantiate(buildingPrefab, request.Position, Quaternion.identity);
        var canPlace = building.CanPlace;
        Destroy(building.gameObject);
        return canPlace;
    }
}