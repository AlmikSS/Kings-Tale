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
        if (!IsServer)
        {
            Destroy(gameObject);
            return;
        }
        
        var gameManager = Instantiate(_gameManagerPrefab);
        var inputManager = Instantiate(_inputManagerPrefab);
        var gameData = Instantiate(_gameDataPrefab);
        var playersData = Instantiate(_playersDataPrefab);
        
        gameData.NetworkObject.Spawn();
        playersData.NetworkObject.Spawn();
        gameManager.NetworkObject.Spawn();
        inputManager.NetworkObject.Spawn();
        
        gameData.Initialize();
        playersData.Initialize();
        gameManager.Initialize();
        inputManager.Initialize();
        
        gameManager.StartGame();
    }
}