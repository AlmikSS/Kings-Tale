using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;

public class GameController : NetworkBehaviour
{
    public static GameController Instance { get; private set; }

    private MatchInfo _currentMatchInfo;
    
    private void Awake()
    {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetMatchSettings(MatchInfo matchInfo)
    {
        _currentMatchInfo = matchInfo;
    }

    public MatchInfo GetMatchInfo()
    {
        return _currentMatchInfo;
    }
}

public struct MatchInfo
{
    public string GameMode;
    public uint PlayersCount;
    public ulong MatchOwnerId;
}