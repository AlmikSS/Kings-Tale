using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

public class InLobbyPlayerObject : MonoBehaviour
{
    [SerializeField] private GameObject _kickBtn;
    [SerializeField] private TMP_Text _nickName;

    private LobbyManagement _lobbyManagement;
    private Player _player;
    
    public void UpdateState(Player player, LobbyManagement lobbyManagement, bool isLobbyOwner)
    {
        _lobbyManagement = lobbyManagement;
        _player = player;
        _nickName.text = player.Data[StartMenuConstants.PLAYER_NICKNAME].Value;
        if (isLobbyOwner)
            _kickBtn.SetActive(true);
    }

    public void Kick() => _lobbyManagement.KickPlayerAsync(_player.Id);
}