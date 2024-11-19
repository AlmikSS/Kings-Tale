using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MineBuilding : WorkingBuilding
{
    [SerializeField] private float _workTime;
    [SerializeField] private ResourcesStruct _resourcesToAdd;

    private GoldenMine _currentGoldenMine;
    
    public override WorkClass GetWork()
    {
        if (!_isBuilt.Value) { return null; }
        
        if (_currentGoldenMine == null || _currentGoldenMine.Cycles <= 0)
            return null;
        
        List<WorkerActionStruct> actions = new();

        var mainBuilding = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(OwnerClientId).GetComponent<PlayerManager>().MainBuilding;
        
        var firstAction = new WorkerActionStruct
        {
            Action = WorkerAction.GoToPoint,
            Target = NetworkObject,
        };
        
        var secondAction = new WorkerActionStruct
        {
            Action = WorkerAction.Wait,
            Target = NetworkObject,
            WaitTime = _workTime,
            WithAction = true
        };

        var thirdAction = new WorkerActionStruct
        {
            Action = WorkerAction.GoToPoint,
            Target = mainBuilding.NetworkObject,
            ResourceToAdd = _resourcesToAdd
        };

        var fourthAction = new WorkerActionStruct
        {
            Action = WorkerAction.Wait,
            Target = mainBuilding.NetworkObject,
            WaitTime = _workTime,
            ResourceToAdd = _resourcesToAdd,
            WithAction = true
        };
        
        actions.Add(firstAction);
        actions.Add(secondAction);
        actions.Add(thirdAction);
        actions.Add(fourthAction);

        _currentWork.Actions = actions;
        return _currentWork;
    }

    [Rpc(SendTo.Owner)]
    public override void PlaceBuildingRpc()
    { 
        _isPlaced.Value = true;
        
        var cols = Physics.OverlapSphere(transform.position, 0.3f);

        foreach (var col in cols)
        {
            if (col.gameObject.TryGetComponent(out GoldenMine mine))
            {
                transform.position = mine.transform.position;
                _currentGoldenMine = mine;
                break;
            }
        }
    }

    public void Mine()
    {
        _currentGoldenMine.Mine();
    }
}