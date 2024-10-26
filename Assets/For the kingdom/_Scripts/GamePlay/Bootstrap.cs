using System.Collections.Generic;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Bootstrap : NetworkBehaviour
{
    [Header("Server")]
    [SerializeField] private GameManager _gameManagerPrefab;
    [SerializeField] private InputManager _inputManagerPrefab;
    [SerializeField] private GameData _gameDataPrefab;
    [SerializeField] private PlayersData _playersDataPrefab;

    [Header("Generation")]
    
    private GameManager _gameManager;
    private InputManager _inputManager;
    private GameData _gameData;
    private PlayersData _playersData;

    private async void Awake()
    {
        DontDestroyOnLoad(this);
        await UnityServices.Instance.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public override void OnNetworkSpawn()
    {
        NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += OnSceneLoadCompleted;
    }

    private void OnSceneLoadCompleted(string scenename, LoadSceneMode loadscenemode, List<ulong> clientscompleted, List<ulong> clientstimedout)
    {
        if (!IsServer) return;
        
        _gameManager = Instantiate(_gameManagerPrefab);
        _inputManager = Instantiate(_inputManagerPrefab);
        _gameData = Instantiate(_gameDataPrefab);
        _playersData = Instantiate(_playersDataPrefab);
        
        _gameManager.NetworkObject.Spawn();
        _inputManager.NetworkObject.Spawn();
        _gameData.NetworkObject.Spawn();
        _playersData.NetworkObject.Spawn();
        
        _gameData.Initialize();
        
        foreach (var client in clientscompleted)
        {
            _playersData.RegisterClient(client);
            Debug.Log("Client registered: " + client);
        }
        
        Init();
    }

    public void Init()
    {
        _gameManager.Initialize(_gameData, _playersData);
        _inputManager.InitializeRpc(_gameManager.NetworkObjectId);
    }
}