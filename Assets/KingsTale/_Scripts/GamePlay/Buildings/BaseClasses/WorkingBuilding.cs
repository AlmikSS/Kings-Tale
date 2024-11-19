using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class WorkingBuilding : Building
{
    [SerializeField] private uint _maxWorkersCount;
    
    protected WorkClass _currentWork = new();
    private NetworkList<ulong> _workers = new(new List<ulong>(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public abstract WorkClass GetWork();

    public bool HasPlace()
    {
        return _workers.Count < _maxWorkersCount;
    }

    public void AddUnit(ulong id)
    {
        if (!IsLocalPlayer) { return; }
        
        if (!_workers.Contains(id))
            _workers.Add(id);
    }

    public void RemoveUnit(ulong id)
    {
        if (!IsLocalPlayer) { return; }
        
        if (_workers.Contains(id))
            _workers.Remove(id);
    }
}