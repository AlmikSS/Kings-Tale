using Unity.Netcode;
using UnityEngine;

public class GoldenMine : NetworkBehaviour
{
    [SerializeField] private int _maxCycles;
    
    private NetworkVariable<int> _cycles = new(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public int Cycles => _cycles.Value;

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }
        
        _cycles.Value = _maxCycles;
    }

    public void Mine()
    {
        _cycles.Value -= 1;
    }
}