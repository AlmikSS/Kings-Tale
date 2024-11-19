using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Worker : UnitBrain
{
    private WorkingBuilding _currentBuilding;
    private WorkClass _currentWork;
    private Building _target;
    private ResourcesStruct _resourcesInInv;
    private WorkerState _currentState = WorkerState.Idle;

    protected override void Update()
    {
        base.Update();

        switch (_currentState)
        {
            case WorkerState.Idle:
                break;
            case WorkerState.GoToTarget:
                GoToTargetUpdateState();
                break;
        }
    }

    private IEnumerator WorkingUpdateStateRoutine()
    {
        _currentState = WorkerState.Working;
        
        foreach (var action in _currentWork.Actions)
        {
            yield return PerformActionRoutine(action);
        }
        
        if (_currentBuilding)
            StartWork();

        _currentState = WorkerState.Idle;
    }

    private IEnumerator PerformActionRoutine(WorkerActionStruct action)
    {
        _resourcesInInv = action.ResourceToAdd;
        _target = action.Target.GetComponent<Building>();
        
        switch (action.Action)
        {
            case WorkerAction.GoToPoint:
                _currentState = WorkerState.GoToTarget;
                GoToPoint(action.Target.transform.position);
                yield return InPath();
                break;
            case WorkerAction.Wait:
                _currentState = WorkerState.Working;
                yield return new WaitForSeconds(action.WaitTime);
                if (action.WithAction)
                {
                    if (_target.TryGetComponent(out Building building))
                    {
                        if (building.IsBuilt)
                        {
                            if (building is MainBuilding)
                            {
                                var request = new ServerAddResourcesRequestStruct
                                {
                                    PlayerId = OwnerClientId,
                                    ResourcesToAdd = _resourcesInInv,
                                };
                                InputManager.Instance.HandleAddResourcesRequestRpc(request);
                            }
                            else if (building is FieldBuilding fieldBuilding)
                                fieldBuilding.Collect();
                            else if (building is MineBuilding mineBuilding)
                                mineBuilding.Mine();
                        }
                        else
                            _target.BuildRpc();
                    }
                }
                break;
            case WorkerAction.Main:
                _currentState = WorkerState.Working;
                yield return new WaitForSeconds(action.WaitTime);
                if (action.Target.TryGetComponent(out Tree tree))
                    tree.MineRpc();
                break;
        }
    }

    private IEnumerator InPath()
    {
        while (_agent.pathPending || _agent.remainingDistance > _agent.stoppingDistance || _agent.velocity.sqrMagnitude > 0f)
        {
            _currentState = WorkerState.GoToTarget;
            yield return null;
        }
    }
    
    private void GoToTargetUpdateState()
    {
        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance && _agent.velocity.sqrMagnitude == 0f)
        {
            if (_currentWork == null)
                StartWork();
        }
    }

    private void StartWork()
    {
        _currentWork = _currentBuilding.GetWork();
        if (_currentWork != null)
            StartCoroutine(WorkingUpdateStateRoutine());
        else 
            ResetWorker();
    }

    [Rpc(SendTo.Owner)]
    public override void SetDestinationRpc(Vector3 point)
    {
        GoToPoint(point);
        ResetWorker();
    }
    
    [Rpc(SendTo.Owner)]
    public override void SetBuildingRpc(ulong buildingId)
    {
        var obj = NetworkManager.Singleton.SpawnManager.SpawnedObjects[buildingId];
        WorkingBuilding workingBuilding = null;

        if (obj.TryGetComponent(out Building building))
        {
            if (!building.IsBuilt)
            {
                _currentWork = building.BuildBuilding();
                StartCoroutine(WorkingUpdateStateRoutine());
            }
            else if (building is WorkingBuilding a)
                workingBuilding = a;
        }
        
        if (workingBuilding == null)
            return;
        
        if (!workingBuilding.HasPlace())
            return;

        ResetWorker();
        
        //GoToPoint(workingBuilding.transform.position);
        
        _currentBuilding = workingBuilding;
        _currentBuilding.AddUnit(NetworkObjectId);
        _currentState = WorkerState.GoToTarget;
    }

    private void ResetWorker()
    {
        if (_currentBuilding != null)
            _currentBuilding.RemoveUnit(NetworkObjectId);
        _target = null;
        _currentBuilding = null;
        _currentWork = null;
        _currentState = WorkerState.Idle;
    }
}

public enum WorkerState
{
    Idle,
    GoToTarget,
    Working
}