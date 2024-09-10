using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;

public class GameData : NetworkBehaviour
{
    public static GameData Instance { get; private set; }

    private Dictionary<ushort, BuildingConfig> _buildingConfigs = new();
    
    public async void Initialize()
    {
        if (!IsServer)
            Destroy(gameObject);
        
        Instance = this;

        if (!await TryGetRemoteConfig())
        {
            GetConfigsFromResources();
        }
    }

    public BuildingConfig GetBuildingConfig(ushort buildingId)
    {
        if (_buildingConfigs.ContainsKey(buildingId))
            return _buildingConfigs[buildingId];
        
        return null;
    }
    
    public bool IsBuildingIdExist(ushort buildingId)
    {
        if (_buildingConfigs.ContainsKey(buildingId))
            return true;

        return false;
    }
    
    private async Task<bool> TryGetRemoteConfig()
    {
        return false;
    }

    private void GetConfigsFromResources()
    {
        _buildingConfigs.Add(0, new BuildingConfig(10, 15, 7));
    }
}