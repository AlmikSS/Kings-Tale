
using System.Threading.Tasks;
using Unity.Services.RemoteConfig;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

class UnitConfig {
 int maxHealth;
 float attackSpeed;
 bool IsLongRange;
 UnitConfig(int maxHealth, float attackSpeed, bool IsLongRange) {
    this.maxHealth = maxHealth;
    this.attackSpeed = attackSpeed;
    this.IsLongRange = IsLongRange;
 }
}


public class Fetcher : MonoBehaviour
{
    public struct userAttributes {}
    public struct appAttributes {}

    async Task InitializeRemoteConfigAsync()
    {
            // initialize handlers for unity game services
            await UnityServices.InitializeAsync();

            // remote config requires authentication for managing environment information
            if (!AuthenticationService.Instance.IsSignedIn)
            {
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
    }

    async Task Start()
    {
        // initialize Unity's authentication and core services, however check for internet connection
        // in order to fail gracefully without throwing exception if connection does not exist
        if (Utilities.CheckForInternetConnection())
        {
            await InitializeRemoteConfigAsync();
        }

        RemoteConfigService.Instance.FetchConfigs(new userAttributes(), new appAttributes());
        ApplyRemoteSettings();
    }

    void ApplyRemoteSettings()
    {
        Debug.Log("RemoteConfigService.Instance.appConfig fetched: " + RemoteConfigService.Instance.appConfig.config.ToString());
    }

    Dictionary<string, UnitConfig> fetch() {
        Dictionary<string, UnitConfig> result = new Dictionary<string, UnitConfig>();
        var cfg = RemoteConfigService.Instance.appConfig.config;
        foreach (KeyValuePair<string, string> kv in cfg) {
            result.Add(kv.Key, new UnitConfig(kv.Value["maxHealth"], kv.Value["attackSpeed"], kv.Value["isLongRange"]));
        }

        return result;
    }
}