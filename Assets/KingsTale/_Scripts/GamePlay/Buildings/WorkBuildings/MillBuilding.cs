using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MillBuilding : WorkingBuilding
{
    [SerializeField] private ResourcesStruct _resourcesToAdd;
    [SerializeField] private LayerMask _buildingLayerMask;
    [SerializeField] private float _seekRange;
    [SerializeField] private float _waitTime;
    
    public override WorkClass GetWork()
    {
        if (!_isBuilt.Value) { return null; }
        
        List<WorkerActionStruct> actions = new();
        
        var mainBuilding = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(OwnerClientId).GetComponent<PlayerManager>().MainBuilding;

        var cols = Physics.OverlapSphere(transform.position, _seekRange, _buildingLayerMask);
        List<FieldBuilding> fields = new();

        foreach (var col in cols)
        {
            if (col.gameObject.TryGetComponent(out FieldBuilding building))
                fields.Add(building);
        }

        FieldBuilding nearField = new();
        
        foreach (var field in fields)
        {
            if (!field.HasAsigned && field.IsGrew)
            {
                nearField = field;
                nearField.Asign();
                break;
            }
        }
        
        var frtAction = new WorkerActionStruct
        {
            Action = WorkerAction.GoToPoint,
            Target = NetworkObject,
        };

        if (nearField == null)
        {
            actions.Add(frtAction);
            _currentWork.Actions = actions;
            return _currentWork;
        }
        
        var sndAction = new WorkerActionStruct
        {
            Action = WorkerAction.GoToPoint,
            Target = nearField.NetworkObject,
        };

        var trdAction = new WorkerActionStruct
        {
            Action = WorkerAction.Wait,
            Target = nearField.NetworkObject,
            WaitTime = _waitTime,
            WithAction = true,
        };

        var fthAction = new WorkerActionStruct
        {
            Action = WorkerAction.GoToPoint,
            Target = NetworkObject,
        };

        var fftAction = new WorkerActionStruct
        {
            Action = WorkerAction.Wait,
            Target = NetworkObject,
            WaitTime = _waitTime
        };

        var sthAction = new WorkerActionStruct
        {
            Action = WorkerAction.GoToPoint,
            Target = mainBuilding.NetworkObject,
            ResourceToAdd = _resourcesToAdd
        };

        var svthAction = new WorkerActionStruct
        {
            Action = WorkerAction.Wait,
            Target = mainBuilding.NetworkObject,
            WaitTime = _waitTime,
            ResourceToAdd = _resourcesToAdd,
            WithAction = true
        };
        
        actions.Add(frtAction);
        actions.Add(sndAction);
        actions.Add(trdAction);
        actions.Add(fthAction);
        actions.Add(fftAction);
        actions.Add(sthAction);
        actions.Add(svthAction);

        _currentWork.Actions = actions;
        
        return _currentWork;
    }
}