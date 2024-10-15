using Unity.Netcode;
using UnityEngine;

public class ServerBootstrap : NetworkBehaviour
{
    [SerializeField] private GameManager _gameManagerPrefab;
    [SerializeField] private InputManager _inputManagerPrefab;
    [SerializeField] private GameData _gameDataPrefab;
    [SerializeField] private PlayersData _playersDataPrefab;
    
    public void Init()
    {
        if (!IsServer)
            { return; }
        
        var gameManager = Instantiate(_gameManagerPrefab);
        var inputManager = Instantiate(_inputManagerPrefab);
        var gameData = Instantiate(_gameDataPrefab);
        var playersData = Instantiate(_playersDataPrefab);
        
        gameManager.NetworkObject.Spawn();
        inputManager.NetworkObject.Spawn();
        // gameData.NetworkObject.Spawn();
        // playersData.NetworkObject.Spawn();
        
        gameData.Initialize();
        playersData.Initialize();
        gameManager.Initialize(gameData, playersData);
        inputManager.InitializeRpc(gameManager.NetworkObjectId);
    }
}