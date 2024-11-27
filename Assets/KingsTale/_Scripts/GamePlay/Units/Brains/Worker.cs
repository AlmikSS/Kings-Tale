using System.Collections;
using Unity.Netcode;
using Unity.Netcode.Components;
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
                if (action.WithAction)
                {
                    // Включаем анимацию работы
                    SetWorkAnimationServerRpc(true);
                }
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
                    // Выключаем анимацию работы
                    SetWorkAnimationServerRpc(false);
                }
                break;
            case WorkerAction.Main:
                _currentState = WorkerState.Working;
                // Включаем анимацию добычи
                SetMineAnimationServerRpc(true);
                yield return new WaitForSeconds(action.WaitTime);
                if (action.Target.TryGetComponent(out Tree tree))
                    tree.MineRpc();
                // Выключаем анимацию добычи
                SetMineAnimationServerRpc(false);
                break;
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

    // RPC для управления анимацией Work
    [ServerRpc(RequireOwnership = false)]
    private void SetWorkAnimationServerRpc(bool isWorking)
    {
        SetWorkAnimationClientRpc(isWorking);
    }

    [ClientRpc]
    private void SetWorkAnimationClientRpc(bool isWorking)
    {
        _networkAnimator.Animator.SetBool(GamePlayConstants.WORK_ANIMATOR_PAR, isWorking);
    }

    // RPC для управления анимацией Mine
    [ServerRpc(RequireOwnership = false)]
    private void SetMineAnimationServerRpc(bool isMining)
    {
        SetMineAnimationClientRpc(isMining);
    }

    [ClientRpc]
    private void SetMineAnimationClientRpc(bool isMining)
    {
        _networkAnimator.Animator.SetBool(GamePlayConstants.MINE_ANIMATOR_PAR, isMining);
    }
}

// Определение WorkerState
public enum WorkerState
{
    Idle,
    GoToTarget,
    Working
}
