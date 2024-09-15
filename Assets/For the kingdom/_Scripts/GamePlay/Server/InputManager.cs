using System.Threading.Tasks;
using Unity.Netcode;

public class InputManager : NetworkBehaviour
{
   public static InputManager Instance { get; private set; }

   private GameManager _gameManager;
   
   public void Initialize(GameManager gameManager)
   {
      if (!IsServer)
         Destroy(gameObject);

      Instance = this;
      _gameManager = gameManager;
   }

   public async Task<InputManagerResponseStruct> HandleBuyRequest(ServerBuyRequestStruct request)
   {
      var validateResponse = await ValidateRequest(request);
      var response = new InputManagerResponseStruct(validateResponse.IsValidate, validateResponse.Message);

      if (validateResponse.IsValidate)
         _gameManager.HandleBuyRequest(request);

      return response;
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