using System.Collections.Generic;
using Unity.Netcode;

public class PlayersData : NetworkBehaviour
{
    private Dictionary<ushort, PlayerDataStruct> _players = new();
    
    public void Initialize()
    {
        
    }

    public PlayerDataStruct? GetPlayer(ushort id)
    {
        if (_players.TryGetValue(id, out var player))
            return player;
        
        return null;
    }
}