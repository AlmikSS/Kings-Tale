using System.Threading.Tasks;
using Unity.Netcode;

public class InputManager : NetworkBehaviour
{
   public static InputManager Instance { get; private set; }

   private NetworkVariable<ulong> _inputManagerId = new();
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
      if (IsServer)
         _gameManager = NetworkManager.Singleton.SpawnManager.SpawnedObjects[gameManagerId].GetComponent<GameManager>();
      
      Instance = NetworkManager.Singleton.SpawnManager.SpawnedObjects[_inputManagerId.Value].GetComponent<InputManager>();
   }
   
   [Rpc(SendTo.Server)]
   public void HandleBuyRequestRpc(ServerBuyRequestStruct request)
   {
      HandleBuyRequestAsync(request);
   }

   [Rpc(SendTo.Server)]
   public void HandlePlaceBuildingRequestRpc(ServerPlaceBuildingRequestStruct request)
   {
      HandlePlaceBuildingRequestAsync(request);
   }

   [Rpc(SendTo.Server)]
   public void HandleAddResourcesRequestRpc(ServerAddResourcesRequestStruct request)
   {
      HandleAddResourcesRequestAsync(request);
   }

   [Rpc(SendTo.Server)]
   public void HandleSetUnitDestinationRequestRpc(ServerSetUnitDestinationRequestStruct request)
   {
      HandleSetUnitDestinationRequestAsync(request);
   }

   [Rpc(SendTo.Server)]
   public void HandleSetUnitBuildingRequestRpc(ServerSetUnitBuildingRequestStruct request)
   {
      HandleSetUnitBuildingRequestAsync(request);
   }

   [Rpc(SendTo.Server)]
   public void HandleTakeDamageRequestRpc(ServerTakeDamageRequestStruct request)
   {
      HandleTakeDamageRequestAsync(request);
   }

   [Rpc(SendTo.Server)]
   public void HandleDieRequestRpc(ServerDieRequestStruct request)
   {
      HandleDieRequestAsync(request);
   }

   [Rpc(SendTo.Server)]
   public void HandleAddUnitsPlaceRpc(ServerAddUnitsPlaceRequestStruct request)
   {
      HandleAddUnitsPlaceAsync(request);
   }
   
   private async void HandleBuyRequestAsync(ServerBuyRequestStruct request)
   {
      if (!IsServer) return;
       
      var validateResponse = await ValidateRequest(request); 
      if (validateResponse.IsValidate)
         _gameManager.HandleBuyRequestRpc(request);
   }

   private async void HandlePlaceBuildingRequestAsync(ServerPlaceBuildingRequestStruct request)
   {
      if (!IsServer) return;
      
      var validateResponse = await ValidateRequest(request); 
      if (validateResponse.IsValidate)
         _gameManager.HandlePlaceBuildingRequestRpc(request);
   }

   private async void HandleAddResourcesRequestAsync(ServerAddResourcesRequestStruct request)
   {
      if (!IsServer) return;
      
      var validateResponse = await ValidateRequest(request); 
      if (validateResponse.IsValidate)
         _gameManager.HandleAddResourcesRequestRpc(request);
   }
   
   private async void HandleSetUnitDestinationRequestAsync(ServerSetUnitDestinationRequestStruct request)
   {
      if (!IsServer) return;
      
      var validateResponse = await ValidateRequest(request);
      if (validateResponse.IsValidate)
         _gameManager.HandleSetUnitDestinationRequestRpc(request);
   }

   private async void HandleSetUnitBuildingRequestAsync(ServerSetUnitBuildingRequestStruct request)
   {
      if (!IsServer) return;
      
      var validateResponse = await ValidateRequest(request);
      if (validateResponse.IsValidate)
         _gameManager.HandleSetUnitBuildingRequestRpc(request);
   }

   private async void HandleTakeDamageRequestAsync(ServerTakeDamageRequestStruct request)
   {
      if (!IsServer) return;
      
      var validateResponse = await ValidateRequest(request);
      if (validateResponse.IsValidate)
         _gameManager.HandleTakeDamageRequestRpc(request);
   }

   private async void HandleDieRequestAsync(ServerDieRequestStruct request)
   {
      if (!IsServer) return;
      
      var validateResponse = await ValidateRequest(request);
      if (validateResponse.IsValidate)
         _gameManager.HandleDieRequestRpc(request);
   }

   private async void HandleAddUnitsPlaceAsync(ServerAddUnitsPlaceRequestStruct request)
   {
      if (!IsServer) return;
      
      var validateResponse = await ValidateRequest(request);
      if (validateResponse.IsValidate)
         _gameManager.HandleAddUnitsPlaceRpc(request);
   }
   
   private Task<ValidateResponseStruct> ValidateRequest(ServerBuyRequestStruct request)
   {
      ValidateResponseStruct response = new ValidateResponseStruct(true, "");
      
      response = IsPlayerExistValidation(request.PlayerId, response);

      if (request.IsBuilding)
         response = IsBuildingPrefabExistValidation(request.Id, response);
      else
      {
         response = IsUnitPrefabExistValidation(request.Id, response);
         response = HavePlaces(request.PlayerId, response);
      }

      return Task.FromResult(response);
   }

   private Task<ValidateResponseStruct> ValidateRequest(ServerPlaceBuildingRequestStruct request)
   {
      ValidateResponseStruct response = new ValidateResponseStruct(true, "");
      
      response = IsPlayerExistValidation(request.PlayerId, response);
      response = IsBuildingPrefabExistValidation(request.BuildingId, response);
      
      return Task.FromResult(response);
   }

   private Task<ValidateResponseStruct> ValidateRequest(ServerAddResourcesRequestStruct request)
   {
      ValidateResponseStruct response = new ValidateResponseStruct(true, "");
      
      response = IsPlayerExistValidation(request.PlayerId, response);
      
      return Task.FromResult(response);
   }

   private Task<ValidateResponseStruct> ValidateRequest(ServerSetUnitDestinationRequestStruct request)
   {
      ValidateResponseStruct response = new ValidateResponseStruct(true, "");
      
      response = IsUnitExistValidation(request.UnitId, response);
      response = IsPlayerExistValidation(request.PlayerId, response);
      response = IsPlayerObject(request.PlayerId, request.UnitId, response);
      
      return Task.FromResult(response);
   }

   private Task<ValidateResponseStruct> ValidateRequest(ServerSetUnitBuildingRequestStruct request)
   {
      ValidateResponseStruct response = new ValidateResponseStruct(true, "");
      
      response = IsUnitExistValidation(request.UnitId, response);
      response = IsPlayerExistValidation(request.PlayerId, response);
      response = IsPlayerObject(request.PlayerId, request.UnitId, response);
      if (!request.IsOwned)
         response = IsPlayerObject(request.PlayerId, request.BuildingId, response);
      
      return Task.FromResult(response);
   }

   private Task<ValidateResponseStruct> ValidateRequest(ServerTakeDamageRequestStruct request)
   {
      ValidateResponseStruct response = new ValidateResponseStruct(true, "");
      
      response = IsPlayerExistValidation(request.PlayerId, response);
      response = IsDamageableObject(request.Id, response);
      
      return Task.FromResult(response);
   }

   private Task<ValidateResponseStruct> ValidateRequest(ServerDieRequestStruct request)
   {
      ValidateResponseStruct response = new ValidateResponseStruct(true, "");
      
      response = IsDamageableObject(request.Id, response);
      
      return Task.FromResult(response);
   }

   private Task<ValidateResponseStruct> ValidateRequest(ServerAddUnitsPlaceRequestStruct request)
   {
      ValidateResponseStruct response = new ValidateResponseStruct(true, "");

      response = IsPlayerExistValidation(request.PlayerId, response);
      
      return Task.FromResult(response);
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
      if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects[objectId].TryGetComponent(out IDamagable damagable))
      {
         response.IsValidate = false;
         response.Message += "Object is not damageable. ";
      }
      
      return response;
   }

   private ValidateResponseStruct IsPlayerObject(ulong playerId, ulong id, ValidateResponseStruct response)
   {
      var obj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[id];

      if (playerId != obj.OwnerClientId)
      {
         response.IsValidate = false;
         response.Message += "Object is not belong player";
      }

      return response;
   }

   private ValidateResponseStruct HavePlaces(ulong playerId, ValidateResponseStruct response)
   {
      if (!_gameManager.HavePlayerPlaces(playerId))
      {
         response.IsValidate = false;
         response.Message += "Player have not place";
      }
      
      return response;
   }
}