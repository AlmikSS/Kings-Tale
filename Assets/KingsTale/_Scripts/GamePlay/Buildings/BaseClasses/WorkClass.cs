using System;
using System.Collections.Generic;
using Unity.Netcode;

//[CreateAssetMenu(menuName = "Worker/Work", fileName = "NewWorkerWork")]
public class WorkClass
{
    public List<WorkerActionStruct> Actions = new();
}

public enum WorkerAction
{
    GoToPoint,
    Wait,
    Main
}

[Serializable]
public struct WorkerActionStruct
{
    public WorkerAction Action;
    public NetworkObject Target;
    public float WaitTime;
    public ResourcesStruct ResourceToAdd;
}