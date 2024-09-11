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
      var validateResponse = await ValidateBuyRequest(request);
      var response = new InputManagerResponseStruct(validateResponse.IsValidate, validateResponse.Message);

      if (validateResponse.IsValidate)
         _gameManager.HandleBuyRequest(request);

      return response;
   }

   private async Task<ValidateResponseStruct> ValidateBuyRequest(ServerBuyRequestStruct request)
   {
      ValidateResponseStruct response = new ValidateResponseStruct(true, "");

      if (!await _gameManager.IsPlayerExist(request.PlayerId))
      {
         response.IsValidate = false;
         response.Message += "Player does not exist. ";
      }

      if (request.IsBuilding)
      {
         if (!await _gameManager.IsBuildingIdExist(request.Id))
         {
            response.IsValidate = false;
            response.Message += "Building does not exist. ";
         }
      }
      else
      {
         if (!await _gameManager.IsUnitIdExist(request.Id))
         {
            response.IsValidate = false;
            response.Message += "Unit does not exist. ";
         }
      }

      return response;
   } 
}