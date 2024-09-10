using System.Threading.Tasks;
using Unity.Netcode;

public class InputManager : NetworkBehaviour
{
    public static InputManager Instance { get; private set; }

    public void Initialize()
    {
        if (!IsServer)
            Destroy(gameObject);
        
        Instance = this;
    }

    public async Task<InputManagerResponseStruct> HandleBuyBuildingRequest(BuyBuildingRequestStruct requestStruct)
    {
        if (!IsServer) { return default; }

        InputManagerResponseStruct responseStruct;
        
        if (!GameManager.Instance.IsPlayerExist(requestStruct.PlayerId))
        {
            responseStruct = new InputManagerResponseStruct("Player does not exist!", false);
            return responseStruct;
        }

        if (!GameManager.Instance.IsBuildingIdExist(requestStruct.BuildingId))
        {
            responseStruct = new InputManagerResponseStruct("Building does not exist!", false);
            return responseStruct;
        }

        BuyRequestStruct buyRequestStruct = new BuyRequestStruct(requestStruct.PlayerId, true, requestStruct.BuildingId);
        GameManager.Instance.HandleBuyRequest(buyRequestStruct);
        
        responseStruct = new InputManagerResponseStruct("Request sent successfully", true);
        return responseStruct;
    }

    public async Task<InputManagerResponseStruct> HandlePlaceBuildingRequest(PlaceBuildingRequestStruct requestStruct)
    {
        if (!IsServer) { return default; }

        InputManagerResponseStruct responseStruct;
        
        if (!GameManager.Instance.IsPlayerExist(requestStruct.PlayerId))
        {
            responseStruct = new InputManagerResponseStruct("Player does not exist!", false);
            return responseStruct;
        }
        
        GameManager.Instance.HandlePlaceBuildingRequest(requestStruct);
        
        responseStruct = new InputManagerResponseStruct("Request sent successfully", true);
        return responseStruct;
    }

    public async Task<InputManagerResponseStruct> HandleBuyUnitRequest(BuyUnitRequestStruct requestStruct)
    {
        if (!IsServer) { return default; }

        InputManagerResponseStruct responseStruct;
        
        if (!GameManager.Instance.IsPlayerExist(requestStruct.PlayerId))
        {
            responseStruct = new InputManagerResponseStruct("Player does not exist!", false);
            return responseStruct;
        }

        if (!GameManager.Instance.IsUnitExist(requestStruct.UnitId))
        {
            responseStruct = new InputManagerResponseStruct("Unit does not exist!", false);
            return responseStruct;
        }
        
        GameManager.Instance.HandleBuyRequest(new BuyRequestStruct(requestStruct.PlayerId, false, requestStruct.UnitId));
        
        responseStruct = new InputManagerResponseStruct("Request sent successfully", true);
        return responseStruct;
    }
}