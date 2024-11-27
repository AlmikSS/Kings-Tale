using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameData : NetworkBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private List<NetworkObject> _unitsPrefabs = new();
    [SerializeField] private List<NetworkObject> _buildingsPrefabs = new();
    [SerializeField] private List<NetworkObject> _projectilesPrefabs = new();
    [SerializeField] private NetworkObject _playerObject;
    [SerializeField] private NetworkObject _mainBuildingPrefab;

    [Header("Configs/Units")]
    [SerializeField] private List<UnitAttackConfigSO> _attackUnitsConfigs = new();
    [SerializeField] private UnitWorkerConfigSO _workerUnitConfig;

    [Header("Configs/Buildings")]
    [SerializeField] private List<BuildingBaseConfigSO> _buildingConfigs = new();

    private readonly Dictionary<ulong, PlayerData> _players = new();
    private readonly Dictionary<ushort, NetworkObject> _buildingPrefabsCache = new();
    private readonly Dictionary<ushort, NetworkObject> _unitPrefabsCache = new();
    private readonly Dictionary<ushort, NetworkObject> _projectilePrefabsCache = new();
    private readonly Dictionary<ushort, ResourcesStruct> _unitPricesCache = new();
    private readonly Dictionary<ushort, ResourcesStruct> _buildingPricesCache = new();
    private readonly HashSet<ulong> _units = new();
    private readonly HashSet<ulong> _buildings = new();

    public List<NetworkObject> BuildingsPrefabs => _buildingsPrefabs;

    private void Awake()
    {
        InitializeCaches();
    }

    public void RegisterClient(ulong clientId)
    {
        if (!IsServer) return;

        try
        {
            var spawnPosition = GetRandomSpawnPosition();
            var playerObject = SpawnPlayerObject(clientId, spawnPosition);
            var mainBuilding = SpawnMainBuilding(clientId, spawnPosition);
            InitializePlayerData(clientId, playerObject, mainBuilding);

            Debug.Log($"Successfully registered client {clientId}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error registering client {clientId}: {e.Message}");
        }
    }

    public PlayerData GetPlayer(ulong id) => 
        _players.TryGetValue(id, out var player) ? player : null;

    public void AddUnitsPlaces(ulong playerId)
    {
        if (_players.TryGetValue(playerId, out var player))
        {
            player.AddUnitsPlace();
        }
    }

    public bool HavePlace(ulong playerId) => 
        _players.TryGetValue(playerId, out var player) && player.HavePlace();

    public ResourcesStruct GetPlayerResources(ulong id) => 
        _players.TryGetValue(id, out var player) ? player.Resources : default;

    public void AddResourcesToPlayer(ulong playerId, ResourcesStruct resourcesToAdd)
    {
        if (_players.TryGetValue(playerId, out var player))
        {
            player.ChangeResources(resourcesToAdd, 0);
        }
    }

    public void RemoveResourcesToPlayer(ulong playerId, ResourcesStruct resourcesToRemove)
    {
        if (_players.TryGetValue(playerId, out var player))
        {
            player.ChangeResources(resourcesToRemove, 1);
        }
    }

    public void AddUnit(ulong id, ulong playerId)
    {
        if (_units.Add(id) && _players.TryGetValue(playerId, out var player))
        {
            player.AddUnit(id);
        }
    }

    public void RemoveUnit(ulong id, ulong playerId)
    {
        if (_units.Remove(id) && _players.TryGetValue(playerId, out var player))
        {
            player.RemoveUnit(id);
        }
    }

    public void AddBuilding(ulong id, ulong playerId)
    {
        if (_buildings.Add(id) && _players.TryGetValue(playerId, out var player))
        {
            player.AddBuilding(id);
        }
    }

    public void RemoveBuilding(ulong id, ulong playerId)
    {
        if (_buildings.Remove(id) && _players.TryGetValue(playerId, out var player))
        {
            player.RemoveBuilding(id);
        }
    }

    public ResourcesStruct GetPrice(ushort id, bool isBuilding)
    {
        if (isBuilding)
        {
            return _buildingPricesCache.TryGetValue(id, out var price) ? price : default;
        }

        return _unitPricesCache.TryGetValue(id, out var unitPrice) ? unitPrice : default;
    }

    public NetworkObject GetBuildingPrefab(ushort id) =>
        _buildingPrefabsCache.TryGetValue(id, out var prefab) ? prefab : null;

    public NetworkObject GetUnitPrefab(ushort id) =>
        _unitPrefabsCache.TryGetValue(id, out var prefab) ? prefab : null;

    public NetworkObject GetProjectilePrefab(ushort id) =>
        _projectilePrefabsCache.TryGetValue(id, out var prefab) ? prefab : null;

    public bool IsUnitExist(ulong id) => _units.Contains(id);

    private void InitializeCaches()
    {
        foreach (var unit in _unitsPrefabs)
        {
            if (unit.TryGetComponent<UnitBrain>(out var brain))
            {
                _unitPrefabsCache[brain.Id] = unit;
            }
        }

        foreach (var building in _buildingsPrefabs)
        {
            if (building.TryGetComponent<Building>(out var buildingComponent))
            {
                _buildingPrefabsCache[buildingComponent.Id] = building;
            }
        }

        foreach (var projectile in _projectilesPrefabs)
        {
            if (projectile.TryGetComponent<Projectile>(out var projectileComponent))
            {
                _projectilePrefabsCache[projectileComponent.Id] = projectile;
            }
        }

        // Cache unit prices
        if (_workerUnitConfig != null)
        {
            _unitPricesCache[0] = _workerUnitConfig.Price;
        }

        foreach (var config in _attackUnitsConfigs)
        {
            _unitPricesCache[config.UnitId] = config.Price;
        }

        // Cache building prices
        foreach (var config in _buildingConfigs)
        {
            _buildingPricesCache[config.Id] = config.Price;
        }
    }

    private Vector3 GetRandomSpawnPosition() =>
        new(Random.Range(-10, 10), 0, Random.Range(-10, 10));

    private NetworkObject SpawnPlayerObject(ulong clientId, Vector3 position)
    {
        var playerObj = Instantiate(_playerObject, position, _playerObject.transform.rotation);
        playerObj.SpawnAsPlayerObject(clientId);
        return playerObj;
    }

    private NetworkObject SpawnMainBuilding(ulong clientId, Vector3 position)
    {
        var mainBuilding = Instantiate(_mainBuildingPrefab, position, _mainBuildingPrefab.transform.rotation);
        mainBuilding.SpawnWithOwnership(clientId);

        if (mainBuilding.TryGetComponent<Building>(out var building))
        {
            building.PlaceBuildingRpc();
            building.BuildRpc();
        }

        return mainBuilding;
    }

    private void InitializePlayerData(ulong clientId, NetworkObject playerObject, NetworkObject mainBuilding)
    {
        if (playerObject.TryGetComponent<PlayerManager>(out var playerManager))
        {
            if (playerManager.TryGetComponent<BuildingSystem>(out var buildingSystem))
            {
                buildingSystem.SetBuildingListRpc(NetworkObjectId);
            }

            playerManager.SetMainBuildingRpc(mainBuilding);

            var playerData = new PlayerData(new ResourcesStruct(100, 100, 100), playerManager);
            _players.Add(clientId, playerData);
            playerData.UpdatePlayer();
        }
    }
}