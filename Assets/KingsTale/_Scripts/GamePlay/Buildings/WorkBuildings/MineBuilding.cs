using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MineBuilding : WorkingBuilding
{
    [SerializeField] private float _workTime;
    [SerializeField] private ResourcesStruct _resourcesToAdd;
    
    public override WorkClass GetWork()
    {
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
            WaitTime = _workTime
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
            WaitTime = _workTime
        };
        
        actions.Add(firstAction);
        actions.Add(secondAction);
        actions.Add(thirdAction);
        actions.Add(fourthAction);

        _currentWork.Actions = actions;
        return _currentWork;
    }
}