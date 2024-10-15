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

   private async void HandleBuyRequestAsync(ServerBuyRequestStruct request)
   {
      if (!IsServer) return;
       
      var validateResponse = await ValidateRequest(request); 
      if (validateResponse.IsValidate)
         _gameManager.HandleBuyRequestRpc(request);
   }

   private Task<ValidateResponseStruct> ValidateRequest(ServerBuyRequestStruct request)
   {
      ValidateResponseStruct response = new ValidateResponseStruct(true, "");
      
      response = IsPlayerExistValidation(request, response);

      if (request.IsBuilding)
         response = IsBuildingExistValidation(request, response);
      else
         response = IsUnitExistValidation(request, response);

      return Task.FromResult(response);
   }

   private Task<ValidateResponseStruct> ValidateRequest(ServerPlaceBuildingRequestStruct request)
   {
      return default;
   }
    
   private ValidateResponseStruct IsPlayerExistValidation(ServerBuyRequestStruct request, ValidateResponseStruct response)
   {
      if (!_gameManager.IsPlayerExist(request.PlayerId))
      {
         response.IsValidate = false;
         response.Message += "Player does not exist. ";
      }

      return response;
   }
   
   private ValidateResponseStruct IsBuildingExistValidation(ServerBuyRequestStruct request, ValidateResponseStruct response)
   {
      if (!_gameManager.IsBuildingIdExist(request.Id))
      {
         response.IsValidate = false;
         response.Message += "Building does not exist. ";
      }

      return response;
   }
   
   private ValidateResponseStruct IsUnitExistValidation(ServerBuyRequestStruct request, ValidateResponseStruct response)
   {
      if (!_gameManager.IsUnitIdExist(request.Id))
      {
         response.IsValidate = false;
         response.Message += "Unit does not exist. ";
      }

      return response;
   }
}