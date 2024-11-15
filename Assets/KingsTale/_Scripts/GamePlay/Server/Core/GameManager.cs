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

        if (CanPlayerBuy(resources, price))
        {
            player = _gameData.GetPlayer(request.PlayerId);
            
            if (request.IsBuilding)
            {
                player.PlayerManager.GetComponent<BuildingSystem>().StartPlacingBuildingRpc(request.Id);
            }
            else
            {
                _gameData.RemoveResourcesToPlayer(request.PlayerId, (ResourcesStruct)price);
                var unit = Instantiate(_gameData.GetUnitPrefab(request.Id), request.Position, Quaternion.identity);
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

        if (CanPlayerBuy(resources, price) & CanPlaceBuilding(request))
        {
            _gameData.RemoveResourcesToPlayer(request.PlayerId, price);
            var building = Instantiate(_gameData.GetBuildingPrefab(request.BuildingId), request.Position, Quaternion.identity);
            building.SpawnWithOwnership(request.PlayerId);
            _gameData.AddBuilding(building.NetworkObjectId, request.PlayerId);
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
        var unit = player.GetUnit(request.UnitId).GetComponent<UnitBrain>();
        
        unit.SetDestinationRpc(request.Point);
    }

    [Rpc(SendTo.Server)]
    public void HandleSetUnitBuildingRequestRpc(ServerSetUnitBuildingRequestStruct request)
    {
        Debug.Log($"Handle set unit building request. Client: {request.PlayerId}, Unit: {request.UnitId}, Building: {request.BuildingId}.");

        var player = _gameData.GetPlayer(request.PlayerId);
        var unit = player.GetUnit(request.UnitId).GetComponent<UnitBrain>();
        
        unit.SetBuildingRpc(request.BuildingId);
    }

    [Rpc(SendTo.Server)]
    public void HandleTakeDamageRequestRpc(ServerTakeDamageRequestStruct request)
    {
        Debug.Log($"Handle take damage request. Client: {request.PlayerId}, Object: {request.Id}, Damage: {request.Damage}.");

        var damageable = NetworkManager.Singleton.SpawnManager.SpawnedObjects[request.Id].GetComponent<IDamagable>();
        damageable.TakeDamage((int)request.Damage, DamageType.Magical);
    }
    
    public bool IsPlayerExist(ulong id)
    {
        var player = _gameData.GetPlayer(id);
        return player != null;
    }

    public bool IsBuildingPrefabExist(ushort id)
    {
        var building = _gameData.GetBuildingPrefab(id);
        return building != null;
    }

    public bool IsUnitPrefabExist(ushort id)
    {
        var unit = _gameData.GetUnitPrefab(id);
        return unit != null;
    }

    public bool IsUnitExist(ulong id)
    {
        return _gameData.IsUnitExist(id);
    }

    private bool CanPlayerBuy(ResourcesStruct resources, ResourcesStruct? price)
    {
        return resources.Wood >= price?.Wood && resources.Gold >= price?.Gold && resources.Food >= price?.Food;
    }

    private bool CanPlaceBuilding(ServerPlaceBuildingRequestStruct request)
    {
        var buildingPrefab = _gameData.GetBuildingPrefab(request.BuildingId);
        var building = Instantiate(buildingPrefab, request.Position, Quaternion.identity);
        var canPlace = building.GetComponent<Building>().CanBuild;
        Destroy(building.gameObject);
        return canPlace;
    }
}