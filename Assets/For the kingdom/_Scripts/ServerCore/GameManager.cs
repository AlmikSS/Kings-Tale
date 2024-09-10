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

        var entities = FindObjectsOfType<GameEntity>();
        foreach (var entity in entities)
        {
            _gameEntities.Add(entity);
        }
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
        Config config = GameData.Instance.GetConfig(requestStruct.Id, requestStruct.IsBuilding);

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
        Debug.Log(requestStruct.PlayerId + "Place");
    }
    
    private void AllowClientBuyRequest(BuyRequestStruct requestStruct)
    {
        Debug.Log(requestStruct.PlayerId + "Buy response");
    }

    private void CancelClientRequest(CancelClientRequestStruct requestStruct)
    {
        // In progress
    }
    
    private void UpdateGameEntitiesTick()
    {
        if (!IsServer) { return; }
        
        foreach (var gameEntity in _gameEntities)
        {
            if (gameEntity == null)
                _gameEntities.Remove(gameEntity);
            else
            {
                gameEntity.UpdateTick();
                Debug.Log("Entity " + gameEntity.name + " has been updated");
            }
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

    public bool IsUnitExist(ushort unitId)
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