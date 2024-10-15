using System.Collections.Generic;
using Unity.Netcode;

public class PlayersData : NetworkBehaviour
{
    private Dictionary<ulong, PlayerDataStruct> _players = new();
    
    public void Initialize()
    {
        
    }

    public PlayerDataStruct? GetPlayer(ulong id)
    {
        if (_players.TryGetValue(id, out var player))
            return player;
        
        return null;
    }
}