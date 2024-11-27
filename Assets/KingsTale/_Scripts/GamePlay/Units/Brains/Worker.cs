using System.Collections;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Represents a worker unit that can perform various tasks and interact with buildings
/// </summary>
public class Worker : UnitBrain
{
    private const float DISTANCE_CHECK_THRESHOLD = 0.1f;
    
    private WorkingBuilding _currentBuilding;
    private WorkClass _currentWork;
    private Building _target;
    private ResourcesStruct _resourcesInInv;
    private WorkerState _currentState = WorkerState.Idle;

    protected override void Update()
    {
        if (!IsSpawned) return;
        
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
            
        if (_currentWork?.Actions == null)
        {
            Debug.LogWarning($"Worker {NetworkObjectId}: No work actions found");
            yield break;
        }

        foreach (var action in _currentWork.Actions)
        {
            yield return PerformActionRoutine(action);
        }
            
        if (_currentBuilding != null)
            StartWork();

        _currentState = WorkerState.Idle;
    }

    private IEnumerator PerformActionRoutine(WorkerActionStruct action)
    {
        _resourcesInInv = action.ResourceToAdd;
        _target = action.Target?.GetComponent<Building>();
            
        switch (action.Action)
        {
            case WorkerAction.GoToPoint:
                yield return HandleGoToPointAction(action);
                break;
                    
            case WorkerAction.Wait:
                yield return HandleWaitAction(action);
                break;
                
            case WorkerAction.Main:
                yield return HandleMainAction(action);
                break;
                
            default:
                Debug.LogWarning($"Worker {NetworkObjectId}: Unknown action type {action.Action}");
                break;
        }
            
        _currentState = WorkerState.Idle;
    }

    private IEnumerator HandleGoToPointAction(WorkerActionStruct action)
    {
        _currentState = WorkerState.GoToTarget;
        Vector3 targetPosition;

        if (action.Target != null)
        {
            targetPosition = action.Target.transform.position;
        }
        else
        {
            var mainBuilding = NetworkManager.Singleton.SpawnManager
                .GetPlayerNetworkObject(OwnerClientId)?.GetComponent<PlayerManager>()?.MainBuilding;

            if (mainBuilding == null)
            {
                Debug.LogError($"Worker {NetworkObjectId}: Main building not found");
                yield break;
            }

            targetPosition = mainBuilding.transform.position;
        }

        GoToPoint(targetPosition);
        yield return InPath();
    }

    private IEnumerator HandleWaitAction(WorkerActionStruct action)
    {
        _currentState = WorkerState.Working;
        yield return new WaitForSeconds(action.WaitTime);

        if (!action.WithAction || _target == null) yield break;

        if (_target.TryGetComponent(out Building building))
        {
            if (building.IsBuilt)
            {
                switch (building)
                {
                    case MainBuilding:
                        HandleMainBuildingAction();
                        break;
                    case FieldBuilding fieldBuilding:
                        fieldBuilding.Collect();
                        break;
                    case MineBuilding mineBuilding:
                        mineBuilding.Mine();
                        break;
                }
            }
            else
            {
                _target.BuildRpc();
            }
        }
    }

    private IEnumerator HandleMainAction(WorkerActionStruct action)
    {
        _currentState = WorkerState.Working;
        yield return new WaitForSeconds(action.WaitTime);

        if (action.Target != null && action.Target.TryGetComponent(out Tree tree))
        {
            tree.MineRpc();
        }
    }

    private void HandleMainBuildingAction()
    {
        var request = new ServerAddResourcesRequestStruct
        {
            PlayerId = OwnerClientId,
            ResourcesToAdd = _resourcesInInv,
        };
        InputManager.Instance.HandleAddResourcesRequestRpc(request);
    }

    private void GoToTargetUpdateState()
    {
        if (!IsPathComplete()) return;
        
        if (_currentWork == null)
            StartWork();
    }

    private bool IsPathComplete()
    {
        return !_agent.pathPending && 
               _agent.remainingDistance <= _agent.stoppingDistance && 
               _agent.velocity.sqrMagnitude <= DISTANCE_CHECK_THRESHOLD;
    }

    private void StartWork()
    {
        if (_currentBuilding == null)
        {
            ResetWorker();
            return;
        }

        _currentWork = _currentBuilding.GetWork();
        if (_currentWork != null)
            StartCoroutine(WorkingUpdateStateRoutine());
        else 
            ResetWorker();
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

    [Rpc(SendTo.Owner)]
    public override void SetDestinationRpc(Vector3 point)
    {
        GoToPoint(point);
        ResetWorker();
    }
    
    [Rpc(SendTo.Owner)]
    public override void SetBuildingRpc(ulong buildingId)
    {
        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(buildingId, out var obj))
        {
            Debug.LogError($"Worker {NetworkObjectId}: Building with ID {buildingId} not found");
            return;
        }

        if (!obj.TryGetComponent(out Building building))
        {
            Debug.LogError($"Worker {NetworkObjectId}: Object with ID {buildingId} is not a building");
            return;
        }

        if (!building.IsBuilt)
        {
            _currentWork = building.BuildBuilding();
            StartCoroutine(WorkingUpdateStateRoutine());
            return;
        }

        if (building is not WorkingBuilding workingBuilding || !workingBuilding.HasPlace())
        {
            Debug.LogWarning($"Worker {NetworkObjectId}: Building {buildingId} is not available for work");
            return;
        }

        ResetWorker();
        
        _currentBuilding = workingBuilding;
        _currentBuilding.AddUnit(NetworkObjectId);
        _currentState = WorkerState.GoToTarget;
    }
}

/// <summary>
/// Represents the possible states of a worker unit
/// </summary>
public enum WorkerState
{
    Idle,
    GoToTarget,
    Working
}