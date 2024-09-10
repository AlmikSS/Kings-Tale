using System.Collections.Generic;
using Unity.Netcode;

public class PlayersData : NetworkBehaviour
{
    public static PlayersData Instance { get; private set; }

    private Dictionary<ushort, PlayerData> _playerDatas = new();
    
    public void Initialize()
    {
        if (!IsServer)
            Destroy(gameObject);
        
        Instance = this;
        
        _playerDatas.Add(0, new PlayerData(20, 50, 20));
    }

    public PlayerData GetPlayerData(ushort playerId)
    {
        if (_playerDatas.ContainsKey(playerId))
            return _playerDatas[playerId];
        
        return null;
    }
}