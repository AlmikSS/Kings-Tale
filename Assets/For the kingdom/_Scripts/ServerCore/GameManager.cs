using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    private List<GameEntity> _gameEntities = new();
    
    public void Initialize()
    {
        if (!IsServer)
            Destroy(gameObject);
        
        Instance = this;
    }

    public void StartGame()
    {
        StartCoroutine(GameCycle());
    }
    
    private IEnumerator GameCycle()
    {
        if (!IsServer) { yield break; }
        
        while (true)
        {
            yield return new WaitForSecondsRealtime(0.01f);
            UpdateGameEntitiesTick();
        }
    }

    public void HandleBuyRequest(BuyRequestStruct requestStruct)
    {
        PlayerData playerData = PlayersData.Instance.GetPlayerData(requestStruct.PlayerId);
        Config config = null;

        if (requestStruct.IsBuilding)
            config = GameData.Instance.GetBuildingConfig(requestStruct.Id);

        if (IsEnaughRes(config, playerData))
            AllowClientBuyRequest(requestStruct);
        else
            CancelClientRequest(new CancelClientRequestStruct(requestStruct.PlayerId));
    }

    public void HandlePlaceBuildingRequest(PlaceBuildingRequestStruct requestStruct)
    {
        if (CanPlaceBuilding(requestStruct))
            AllowClientPlaceBuilding(requestStruct);
        else
            CancelClientRequest(new CancelClientRequestStruct(requestStruct.PlayerId));
    }

    private void AllowClientPlaceBuilding(PlaceBuildingRequestStruct requestStruct)
    {
        // Дописать
    }
    
    private void AllowClientBuyRequest(BuyRequestStruct requestStruct)
    {
        // Дописать
    }

    private void CancelClientRequest(CancelClientRequestStruct requestStruct)
    {
        // Дописать
    }
    
    private void UpdateGameEntitiesTick()
    {
        if (!IsServer) { return; }
        
        foreach (var gameEntity in _gameEntities)
        {
            gameEntity.UpdateTick();
            Debug.Log("Entity " + gameEntity.name + " has been updated");
        }
    }
    
    public bool IsBuildingIdExist(ushort buildingId)
    {
        return GameData.Instance.IsBuildingIdExist(buildingId);
    }
    
    public bool IsPlayerExist(ushort playerId)
    {
        return true;
    }
    
    private bool IsEnaughRes(Config config, PlayerData playerData)
    {
        bool isEnaughWood = false;
        bool isEnaughGold = false;
        bool isEnaughFood = false;

        if (playerData.Wood >= config.WoodPrice)
            isEnaughWood = true;
        
        if (playerData.Gold >= config.GoldPrice)
            isEnaughGold = true;
        
        if (playerData.Food >= config.FoodPrice)
            isEnaughFood = true;

        return isEnaughWood && isEnaughGold && isEnaughFood;
    }

    private bool CanPlaceBuilding(PlaceBuildingRequestStruct requestStruct)
    {
        return true;
    }
}