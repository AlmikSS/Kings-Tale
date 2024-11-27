using System.Collections.Generic;
using Unity.Netcode;

public class Tree : WorkingBuilding
{
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }
        
        base.OnNetworkSpawn();
        
        _isBuilt.Value = true;
    }
    
    public override WorkClass GetWork()
    {
        List<WorkerActionStruct> actions = new();

        var firstAction = new WorkerActionStruct
        {
            Action = WorkerAction.GoToPoint,
            Target = NetworkObject
        };

        var secondAction = new WorkerActionStruct
        {
            Action = WorkerAction.Main,
            WaitTime = _workTime,
            Target = NetworkObject,
            WithAction = true
        };

        var thirdAction = new WorkerActionStruct
        {
            Action = WorkerAction.GoToPoint,
            ResourceToAdd = _resourcesToAdd
        };

        var fthAction = new WorkerActionStruct
        {
            Action = WorkerAction.Wait,
            WaitTime = _restTime,
            WithAction = true,
            ResourceToAdd = _resourcesToAdd
        };
        
        actions.Add(firstAction);
        actions.Add(secondAction);
        actions.Add(thirdAction);
        actions.Add(fthAction);
        
        _currentWork.Actions = actions;

        return _currentWork;
    }

    private NetworkObject GetNearStorage()
    {
        var mainBuilding = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(OwnerClientId).GetComponent<PlayerManager>().MainBuilding;

        return mainBuilding.NetworkObject;
    }

    [Rpc(SendTo.Everyone)]
    public void MineRpc()
    {
        _currentHealth.Value -= 1;

        if (_currentHealth.Value <= 0)
        {
            var request = new ServerDieRequestStruct
            {
                IsBuilding = true,
                Id = NetworkObjectId
            };
            
            InputManager.Instance.HandleDieRequestRpc(request);
        }
    }   
    
    public override void TakeDamageRpc(int damage)
    {
        
    }
}