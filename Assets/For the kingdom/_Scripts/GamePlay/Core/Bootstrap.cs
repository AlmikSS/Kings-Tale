using Unity.Netcode;
using UnityEngine;

public class Bootstrap : NetworkBehaviour
{
    [SerializeField] private GameManager _gameManagerPrefab;
    [SerializeField] private InputManager _inputManagerPrefab;
    [SerializeField] private GameData _gameDataPrefab;
    [SerializeField] private PlayersData _playersDataPrefab;
    
    public override void OnNetworkSpawn()
    {
        var gameManager = Instantiate(_gameManagerPrefab);
        var inputManager = Instantiate(_inputManagerPrefab);
        var gameData = Instantiate(_gameDataPrefab);
        var playersData = Instantiate(_playersDataPrefab);

        gameData.Initialize();
        playersData.Initialize();
        gameManager.Initialize(gameData, playersData);
        inputManager.Initialize(gameManager);
        
        gameManager.NetworkObject.Spawn();
        inputManager.NetworkObject.Spawn();
        gameData.NetworkObject.Spawn();
        playersData.NetworkObject.Spawn();
    }
}