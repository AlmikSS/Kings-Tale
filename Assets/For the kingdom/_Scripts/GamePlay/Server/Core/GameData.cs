using Unity.Netcode;
using UnityEngine;

public class GameData : NetworkBehaviour
{
    public void Initialize()
    {
        GetConfigs();
    }
    
    private void GetConfigs()
    {
        var configs = Resources.LoadAll("Configs/Units");
        Debug.Log(configs);
    }

    public ResourcesStruct? GetPrice(uint id, bool isBuilding)
    {
        return default;
    }

    public BuildingBase GetBuilding(ushort id)
    {
        return default;
    }

    public GameObject GetUnit(ushort id)
    {
        return default;
    }
}