using System;
using System.Collections.Generic;
using Unity.Netcode;

[Serializable]
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
    public ResourcesStruct ResourceToAdd;
    public bool WithAction;
    public float WaitTime;
}