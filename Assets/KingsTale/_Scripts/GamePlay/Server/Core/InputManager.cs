using System;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class InputManager : NetworkBehaviour
{
    public static InputManager Instance { get; private set; }

    private readonly NetworkVariable<ulong> _inputManagerId = new();
    private GameManager _gameManager;

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            _inputManagerId.Value = NetworkObjectId;
        }
    }
    
    [Rpc(SendTo.Everyone)]
    public void InitializeRpc(ulong gameManagerId)
    { 
        if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(gameManagerId, out var gameManagerObject))
        {
            Debug.LogError($"GameManager with ID {gameManagerId} not found!");
            return;
        }

        if (IsServer)
        {
            _gameManager = gameManagerObject.GetComponent<GameManager>();
            if (_gameManager == null)
            {
                Debug.LogError("GameManager component not found on the object!");
                return;
            }
        }

        if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(_inputManagerId.Value, out var inputManagerObject))
        {
            Debug.LogError($"InputManager with ID {_inputManagerId.Value} not found!");
            return;
        }

        Instance = inputManagerObject.GetComponent<InputManager>();
    }

    [Rpc(SendTo.Server)]
    public void HandleBuyRequestRpc(ServerBuyRequestStruct request) => 
        HandleRequestAsync(request, _gameManager.HandleBuyRequestRpc);

    [Rpc(SendTo.Server)]
    public void HandlePlaceBuildingRequestRpc(ServerPlaceBuildingRequestStruct request) => 
        HandleRequestAsync(request, _gameManager.HandlePlaceBuildingRequestRpc);

    [Rpc(SendTo.Server)]
    public void HandleAddResourcesRequestRpc(ServerAddResourcesRequestStruct request) => 
        HandleRequestAsync(request, _gameManager.HandleAddResourcesRequestRpc);

    [Rpc(SendTo.Server)]
    public void HandleSetUnitDestinationRequestRpc(ServerSetUnitDestinationRequestStruct request) => 
        HandleRequestAsync(request, _gameManager.HandleSetUnitDestinationRequestRpc);

    [Rpc(SendTo.Server)]
    public void HandleSetUnitBuildingRequestRpc(ServerSetUnitBuildingRequestStruct request) => 
        HandleRequestAsync(request, _gameManager.HandleSetUnitBuildingRequestRpc);

    [Rpc(SendTo.Server)]
    public void HandleTakeDamageRequestRpc(ServerTakeDamageRequestStruct request) => 
        HandleRequestAsync(request, _gameManager.HandleTakeDamageRequestRpc);

    [Rpc(SendTo.Server)]
    public void HandleDieRequestRpc(ServerDieRequestStruct request) => 
        HandleRequestAsync(request, _gameManager.HandleDieRequestRpc);

    [Rpc(SendTo.Server)]
    public void HandleAddUnitsPlaceRequestRpc(ServerAddUnitsPlaceRequestStruct request) => 
        HandleRequestAsync(request, _gameManager.HandleAddUnitsPlaceRpc);

    [Rpc(SendTo.Server)]
    public void HandleSpawnProjectileRequestRpc(ServerSpawnProjectileRequestStruct request) => 
        HandleRequestAsync(request, _gameManager.HandleSpawnProjectileRequestRpc);

    [Rpc(SendTo.Server)]
    public void HandleDespawnRequestRpc(ServerDespawnRequestStruct request) => 
        HandleRequestAsync(request, _gameManager.HandleDespawnRequestRpc);

    private async void HandleRequestAsync<T>(T request, Action<T> handler)
    {
        try
        {
            if (!IsServer)
            {
                Debug.LogWarning("Request received on non-server instance");
                return;
            }

            if (_gameManager == null)
            {
                Debug.LogError("GameManager is not initialized!");
                return;
            }

            var validateResponse = await ValidateRequest(request);
            if (validateResponse.IsValidate)
            {
                handler(request);
            }
            else
            {
                Debug.LogWarning($"Request validation failed: {validateResponse.Message}");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Error handling request: {e.Message}");
        }
    }

    private Task<ValidateResponseStruct> ValidateRequest<T>(T request)
    {
        var response = new ValidateResponseStruct(true, string.Empty);

        switch (request)
        {
            case ServerBuyRequestStruct buyRequest:
                ValidateBuyRequest(buyRequest, ref response);
                break;

            case ServerPlaceBuildingRequestStruct placeBuildingRequest:
                ValidatePlaceBuildingRequest(placeBuildingRequest, ref response);
                break;

            case ServerAddResourcesRequestStruct addResourcesRequest:
                response = IsPlayerExistValidation(addResourcesRequest.PlayerId, response);
                break;

            case ServerSetUnitDestinationRequestStruct unitDestRequest:
                ValidateUnitRequest(unitDestRequest.PlayerId, unitDestRequest.UnitId, ref response);
                break;

            case ServerSetUnitBuildingRequestStruct unitBuildRequest:
                ValidateUnitRequest(unitBuildRequest.PlayerId, unitBuildRequest.UnitId, ref response);
                break;

            case ServerTakeDamageRequestStruct damageRequest:
                ValidateDamageRequest(damageRequest, ref response);
                break;

            case ServerDieRequestStruct dieRequest:
                response = IsDamageableObject(dieRequest.Id, response);
                break;

            case ServerAddUnitsPlaceRequestStruct unitsPlaceRequest:
                response = IsPlayerExistValidation(unitsPlaceRequest.PlayerId, response);
                break;

            case ServerSpawnProjectileRequestStruct projectileRequest:
                response = IsProjectileExist(projectileRequest.ProjectileId, response);
                break;

            case ServerDespawnRequestStruct despawnRequest:
                ValidateDespawnRequest(despawnRequest, ref response);
                break;
        }

        return Task.FromResult(response);
    }

    private void ValidateBuyRequest(ServerBuyRequestStruct request, ref ValidateResponseStruct response)
    {
        response = IsPlayerExistValidation(request.PlayerId, response);

        if (request.IsBuilding)
        {
            response = IsBuildingPrefabExistValidation(request.Id, response);
        }
        else
        {
            response = IsUnitPrefabExistValidation(request.Id, response);
            response = HavePlaces(request.PlayerId, response);
        }
    }

    private void ValidatePlaceBuildingRequest(ServerPlaceBuildingRequestStruct request, ref ValidateResponseStruct response)
    {
        response = IsPlayerExistValidation(request.PlayerId, response);
        response = IsBuildingPrefabExistValidation(request.BuildingId, response);
    }

    private void ValidateUnitRequest(ulong playerId, ulong unitId, ref ValidateResponseStruct response)
    {
        response = IsUnitExistValidation(unitId, response);
        response = IsPlayerExistValidation(playerId, response);
        response = IsPlayerObject(playerId, unitId, response);
    }

    private void ValidateDamageRequest(ServerTakeDamageRequestStruct request, ref ValidateResponseStruct response)
    {
        response = IsPlayerExistValidation(request.PlayerId, response);
        response = IsDamageableObject(request.Id, response);
    }

    private void ValidateDespawnRequest(ServerDespawnRequestStruct request, ref ValidateResponseStruct response)
    {
        if (!NetworkManager.SpawnManager.SpawnedObjects.ContainsKey(request.Id))
        {
            response.IsValidate = false;
            response.Message += "Object does not exist. ";
        }
    }

    private ValidateResponseStruct IsPlayerExistValidation(ulong playerId, ValidateResponseStruct response)
    {
        if (!_gameManager.IsPlayerExist(playerId))
        {
            response.IsValidate = false;
            response.Message += "Player does not exist. ";
        }
        return response;
    }
   
    private ValidateResponseStruct IsBuildingPrefabExistValidation(ushort buildingId, ValidateResponseStruct response)
    {
        if (!_gameManager.IsBuildingPrefabExist(buildingId))
        {
            response.IsValidate = false;
            response.Message += "Building prefab does not exist. ";
        }
        return response;
    }
   
    private ValidateResponseStruct IsUnitPrefabExistValidation(ushort unitId, ValidateResponseStruct response)
    {
        if (!_gameManager.IsUnitPrefabExist(unitId))
        {
            response.IsValidate = false;
            response.Message += "Unit prefab does not exist. ";
        }
        return response;
    }

    private ValidateResponseStruct IsUnitExistValidation(ulong unitId, ValidateResponseStruct response)
    {
        if (!_gameManager.IsUnitExist(unitId))
        {
            response.IsValidate = false;
            response.Message += "Unit does not exist. ";
        }
        return response;
    }

    private ValidateResponseStruct IsDamageableObject(ulong objectId, ValidateResponseStruct response)
    {
        if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(objectId, out var spawnedObject) || 
            !spawnedObject.TryGetComponent(out IDamagable _))
        {
            response.IsValidate = false;
            response.Message += "Object is not damageable. ";
        }
        return response;
    }

    private ValidateResponseStruct IsPlayerObject(ulong playerId, ulong id, ValidateResponseStruct response)
    {
        if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(id, out var obj) || 
            playerId != obj.OwnerClientId)
        {
            response.IsValidate = false;
            response.Message += "Object does not belong to player. ";
        }
        return response;
    }

    private ValidateResponseStruct HavePlaces(ulong playerId, ValidateResponseStruct response)
    {
        if (!_gameManager.HavePlayerPlaces(playerId))
        {
            response.IsValidate = false;
            response.Message += "Player has no available places. ";
        }
        return response;
    }

    private ValidateResponseStruct IsProjectileExist(ushort id, ValidateResponseStruct response)
    {
        if (!_gameManager.IsProjectilePrefabExist(id))
        {
            response.IsValidate = false;
            response.Message += "Projectile does not exist. ";
        }
        return response;
    }
}