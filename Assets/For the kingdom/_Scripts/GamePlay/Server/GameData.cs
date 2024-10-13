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
}