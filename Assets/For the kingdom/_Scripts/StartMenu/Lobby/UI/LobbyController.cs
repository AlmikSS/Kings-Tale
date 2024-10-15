using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyController : MonoBehaviour
{
    [Header("Lobby creating")]
    [SerializeField] private Slider _maxPlayerSlider;
    [SerializeField] private TMP_Dropdown _gameModeDropdown;
    private bool _isPrivateLobby;

    [Header("Lobby joining")]
    [SerializeField] private TMP_InputField _lobbyJoinCodeInputField;
    
    [Header("Options")]
    [SerializeField] private TMP_InputField _playerNickNameInputField;
    [SerializeField] private InLobbyPlayerObject _inLobbyPlayerObjectPrefab;

    [Header("InLobbyMenu")]
    [SerializeField] private GameObject _inLobbyMenuOrigin;
    [SerializeField] private GameObject _inLobbyStartGameBtn; 
    [SerializeField] private TMP_Text _inLobbyMaxPlayersText;
    [SerializeField] private TMP_Text _inLobbyMenuCodeText;
    [SerializeField] private Transform _inLobbyPlayersViewTransform;
    
    private LobbyManager _lobbyManager;
    private LobbyManagement _lobbyManagement;
    private bool _inLobby;
    
    private void Start()
    {
        _lobbyManager = new LobbyManager();
        _lobbyManagement = new LobbyManagement();
        
        _lobbyManager.OpenInLobbyMenuEvent += OpenInLobbyMenu;
        _lobbyManager.CloseInLobbyMenuEvent += CloseInLobbyMenu;
        _lobbyManagement.OnLobbyUpdateEvent += OnLobbyUpdated;
    }
    
    public void CreateLobby()
    {
        if (_inLobby) return;
        
        var maxPlayers = Mathf.RoundToInt(_maxPlayerSlider.value);
        var gameMode = _gameModeDropdown.value.ToString();
        var playerNickName = _playerNickNameInputField.text;
        _lobbyManager.CreateLobbyAsync(maxPlayers, gameMode, playerNickName, _isPrivateLobby);
    }

    public void JoinLobby()
    {
        if (_inLobby) return;
        
        var lobbyCode = _lobbyJoinCodeInputField.text;
        var playerNickName = _playerNickNameInputField.text;
        _lobbyManager.JoinLobbyAsync(lobbyCode, playerNickName);
    }
    
    private void OpenInLobbyMenu(Lobby lobby, bool isLobbyOwner)
    {
        _lobbyManagement.SetLobby(lobby, isLobbyOwner);
        _inLobby = true;
        _inLobbyMenuOrigin.SetActive(true);
        if (isLobbyOwner)
            _inLobbyStartGameBtn.SetActive(true);
    }
    
    private void CloseInLobbyMenu()
    {
        _inLobby = false;
        _inLobbyMenuOrigin.SetActive(false);
        _inLobbyStartGameBtn.SetActive(false);
        _lobbyManagement.DeleteLobby();
    }

    private void OnLobbyUpdated(Lobby lobby, bool isLobbyOwner)
    {
        _inLobbyMaxPlayersText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
        _inLobbyMenuCodeText.text = $"Code: {lobby.LobbyCode}";
        
        foreach (Transform child in _inLobbyPlayersViewTransform)
            Destroy(child.gameObject);

        foreach (var player in lobby.Players)
        {
            var playerObject = Instantiate(_inLobbyPlayerObjectPrefab, _inLobbyPlayersViewTransform);
            playerObject.UpdateState(player, _lobbyManagement, isLobbyOwner);
        }
    }
}