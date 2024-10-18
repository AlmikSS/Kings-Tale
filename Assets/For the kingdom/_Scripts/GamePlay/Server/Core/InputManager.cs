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
   
   private Task<ValidateResponseStruct> ValidateRequest(ServerBuyRequestStruct request)
   {
      ValidateResponseStruct response = new ValidateResponseStruct(true, "");
      
      response = IsPlayerExistValidation(request.PlayerId, response);

      if (request.IsBuilding)
         response = IsBuildingExistValidation(request.Id, response);
      else
         response = IsUnitExistValidation(request.Id, response);

      return Task.FromResult(response);
   }

   private Task<ValidateResponseStruct> ValidateRequest(ServerPlaceBuildingRequestStruct request)
   {
      ValidateResponseStruct response = new ValidateResponseStruct(true, "");
      
      response = IsPlayerExistValidation(request.PlayerId, response);
      response = IsBuildingExistValidation(request.BuildingId, response);
      
      return Task.FromResult(response);
   }

   private Task<ValidateResponseStruct> ValidateRequest(ServerAddResourcesRequestStruct request)
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
   
   private ValidateResponseStruct IsBuildingExistValidation(ushort buildingId, ValidateResponseStruct response)
   {
      if (!_gameManager.IsBuildingIdExist(buildingId))
      {
         response.IsValidate = false;
         response.Message += "Building does not exist. ";
      }

      return response;
   }
   
   private ValidateResponseStruct IsUnitExistValidation(ushort unitId, ValidateResponseStruct response)
   {
      if (!_gameManager.IsUnitIdExist(unitId))
      {
         response.IsValidate = false;
         response.Message += "Unit does not exist. ";
      }

      return response;
   }
}