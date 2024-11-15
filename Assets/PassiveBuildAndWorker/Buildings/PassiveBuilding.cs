using System.Collections.Generic;
using Mono.Cecil;
using UnityEngine;

public class PassiveBuilding : Building
{
    [SerializeField] private List<GameObject> _workersListInBuild = new List<GameObject>();
    [SerializeField] private List<GameObject> _resourcesAround = new List<GameObject>();
    [SerializeField] private ResourceTypeEnum _resourceGatherType;

    public void AddWorker(GameObject worker)
    {
        _workersListInBuild.Add(worker);
    }

    public void TransferWorkerToWork(Worker work)
    {
        for (int i = 0; i < _resourcesAround.Count; i++)
        {
            if(_resourcesAround[i] != null) { if(_resourcesAround[i].GetComponent<ResourceBuilding>().attachedUnit == null)work.ChangeWorkPlace(_resourcesAround[i]); }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Resource") && other.GetComponent<ResourceBuilding>().GetResourceType() == _resourceGatherType)
        {
            _resourcesAround.Add(other.gameObject);
        }
    }
}
