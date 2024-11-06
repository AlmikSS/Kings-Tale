using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    private GameData _gameData;
    
    public void Initialize(GameData gameData)
    {
        _gameData = gameData;
    }

    [Rpc(SendTo.Server)]
    public void HandleBuyRequestRpc(ServerBuyRequestStruct request)
    {
        Debug.Log($"Handle buy request. Client: {request.PlayerId}, Id: {request.Id}, IsBuilding: {request.IsBuilding}.");

        var resources = _gameData.GetPlayerResources(request.PlayerId);
        var price = _gameData.GetPrice(request.Id, request.IsBuilding);
        var player = _gameData.GetPlayer(request.PlayerId);

        //if (resources == null || price == null || player == null) return;

        //if (CanPlayerBuy(resources, price))
        if (true)
        {
            player = _gameData.GetPlayer(request.PlayerId);
            
            if (request.IsBuilding)
            {
                player.PlayerManager.GetComponent<BuildingSystem>().StartPlacingBuildingRpc(request.Id);
            }
            else
            {
                _gameData.RemoveResourcesToPlayer(request.PlayerId, (ResourcesStruct)price);
                var unit = Instantiate(_gameData.GetUnit(request.Id), request.Position, Quaternion.identity);
                unit.SpawnWithOwnership(request.PlayerId);
                _gameData.AddUnit(unit.NetworkObjectId, request.PlayerId);
            }
        }
    }

    [Rpc(SendTo.Server)]
    public void HandlePlaceBuildingRequestRpc(ServerPlaceBuildingRequestStruct request)
    {
        Debug.Log($"Handle place building request. Client: {request.PlayerId}, Building: {request.BuildingId}, Position: {request.Position}.");
        
        var resources = _gameData.GetPlayerResources(request.PlayerId);
        var price = _gameData.GetPrice(request.BuildingId, true);
        var player = _gameData.GetPlayer(request.PlayerId);

        //if (resources == null || price == null || player == null) return;

        //if (CanPlayerBuy(resources, price) && CanPlaceBuilding(request))
        if (true)
        {
            Debug.Log(0);
            //_gameData.RemoveResourcesToPlayer(request.PlayerId, (ResourcesStruct)price);
            var building = Instantiate(_gameData.GetBuilding(request.BuildingId), request.Position, Quaternion.identity);
            building.SpawnWithOwnership(request.PlayerId);
           // _gameData.AddBuilding(building.NetworkObjectId, request.PlayerId);
            player.PlayerManager.GetComponent<BuildingSystem>().OnBuildingPlacedRpc();
        }
    }

    [Rpc(SendTo.Server)]
    public void HandleAddResourcesRequestRpc(ServerAddResourcesRequestStruct request)
    {
        Debug.Log($"Handle add resources request. Client: {request.PlayerId}, Wood: {request.ResourcesToAdd.Wood}, Gold: {request.ResourcesToAdd.Gold}, Food: {request.ResourcesToAdd.Food}.");
        var player = _gameData.GetPlayer(request.PlayerId);

        if (player == null) return;
        
        _gameData.AddResourcesToPlayer(request.PlayerId, request.ResourcesToAdd);
    }

    [Rpc(SendTo.Server)]
    public void HandleSetUnitDestinationRequestRpc(ServerSetUnitDestinationRequestStruct request)
    {
        Debug.Log($"Handle set unit destination request. Client: {request.PlayerId}, Unit: {request.UnitId}, Point: {request.Point}.");

        var player = _gameData.GetPlayer(request.PlayerId);
    }
    
    public bool IsPlayerExist(ulong id)
    {
        var player = _gameData.GetPlayer(id);
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
        var canPlace = building.GetComponent<Building>().CanBuild;
        Destroy(building.gameObject);
        return canPlace;
    }
}