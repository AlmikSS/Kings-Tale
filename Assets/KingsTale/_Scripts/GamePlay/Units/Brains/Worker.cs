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
        foreach (var action in _currentWork.Actions)
        {
            yield return PerformActionRoutine(action);
        }
        
        if (_currentBuilding)
            StartWork();
    }

    private IEnumerator PerformActionRoutine(WorkerActionStruct action)
    {
        _resourcesInInv = action.ResourceToAdd;
        
        switch (action.Action)
        {
            case WorkerAction.GoToPoint:
                _currentState = WorkerState.GoToTarget;
                _agent.SetDestination(action.Target.transform.position);
                _target = action.Target.GetComponent<Building>();
                yield return InPath();
                break;
            case WorkerAction.Wait:
                yield return new WaitForSeconds(action.WaitTime);
                break;
            case WorkerAction.Main:
                break;
        }
    }

    private IEnumerator InPath()
    {
        while (_agent.pathPending || _agent.remainingDistance > _agent.stoppingDistance || _agent.velocity.sqrMagnitude > 0f)
        {
            yield return null;
        }
    }
    
    private void GoToTargetUpdateState()
    {
        if (!_agent.pathPending && _agent.remainingDistance <= _agent.stoppingDistance && _agent.velocity.sqrMagnitude == 0f)
        {
            _currentState = WorkerState.Working;

            if (_currentWork == null)
                StartWork();

            if (_target != null)
            {
                if (_target.TryGetComponent(out Building building))
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
                    
                    if (building is FieldBuilding fieldBuilding)
                        fieldBuilding.Collect();
                }
            }
        }
    }

    private void StartWork()
    {
        _currentWork = _currentBuilding.GetWork();
        StartCoroutine(WorkingUpdateStateRoutine());
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
        var point = NetworkManager.Singleton.SpawnManager.SpawnedObjects[buildingId].transform.position;
        GoToPoint(point);
        
        var building = NetworkManager.Singleton.SpawnManager.SpawnedObjects[buildingId].GetComponent<WorkingBuilding>();

        if (!building.HasPlace())
            return;

        ResetWorker();
        
        _currentBuilding = building;
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
    }
}

public enum WorkerState
{
    Idle,
    GoToTarget,
    Working
}