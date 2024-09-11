using Unity.Netcode;
using UnityEngine;

public class Bootstrap : NetworkBehaviour
{
    [SerializeField] private GameManager _gameManagerPrefab;
    [SerializeField] private InputManager _inputManagerPrefab;
    
    public override void OnNetworkSpawn()
    {
        var gameManager = Instantiate(_gameManagerPrefab);
        var inputManager = Instantiate(_inputManagerPrefab);
        
        gameManager.Initialize();
        inputManager.Initialize(gameManager);
        
        gameManager.NetworkObject.Spawn();
        inputManager.NetworkObject.Spawn();
    }
}