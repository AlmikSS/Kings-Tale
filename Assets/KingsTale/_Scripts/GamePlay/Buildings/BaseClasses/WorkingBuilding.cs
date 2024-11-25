using System.Collections.Generic;
using Unity.Netcode;

//721283.50

public abstract class WorkingBuilding : Building
{
    protected WorkClass _currentWork = new();
    protected ResourcesStruct _resourcesToAdd;
    protected float _workTime;
    protected float _workRange;
    protected float _restTime;
    private uint _maxWorkersCount;
    private NetworkList<ulong> _workers = new(new List<ulong>(), NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public abstract WorkClass GetWork();

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        
        if (!IsOwner) { return; }

        _resourcesToAdd = ((WorkingBuildingConfigSO)_config).ResourcesToAdd;
        _workTime = ((WorkingBuildingConfigSO)_config).WorkTime;
        _workRange = ((WorkingBuildingConfigSO)_config).WorkRange;
        _restTime = ((WorkingBuildingConfigSO)_config).RestTime;
        _maxWorkersCount = ((WorkingBuildingConfigSO)_config).MaxWorkersCount;
    }
    
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