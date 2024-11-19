using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Tree : WorkingBuilding
{
    [SerializeField] private ResourcesStruct _resourcesToAdd;
    [SerializeField] private float _mainTime;
    
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
            WaitTime = _mainTime,
            Target = NetworkObject,
            WithAction = true
        };

        var nearest = GetNearStorage();

        var thirdAction = new WorkerActionStruct
        {
            Action = WorkerAction.GoToPoint,
            Target = nearest,
            ResourceToAdd = _resourcesToAdd
        };

        var fthAction = new WorkerActionStruct
        {
            Action = WorkerAction.Wait,
            Target = nearest,
            WaitTime = _mainTime,
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
    
    public override void TakeDamage(int damage, DamageType type = 0, Effect fx = Effect.None)
    {
        
    }
}