using System;
using UnityEngine;

public class Worker : MonoBehaviour //UnitBrain
{
    [SerializeField] private ResourcesStruct playerResources;
    [SerializeField] private uint amountResourceGather;
    [SerializeField] private GameObject currentWorkPlace;
    [SerializeField] private ResourceBuilding currentWorkPlaceScript;
    [SerializeField] private GameObject currentWarehouse;
    [SerializeField] private PassiveBuilding currentWarehouseScript;
    [SerializeField] private ResourceTypeEnum currentResource;
    
    private bool _workEnded = false, _gotToWorkPlace = false;
    private float _timer = 0, _workTime = 0;

    private void Update()
    {
        if (currentWorkPlace == null) //&& currentWarehouse != Mainbuilding
        {
            currentWarehouseScript.TransferWorkerToWork(this);
        }
        else
        {
            _gotToWorkPlace = false;
            _workEnded = false;
        }
        //Working timer (Amount of time to gather resource)
        if (_gotToWorkPlace && !_workEnded)
        {
            _timer += Time.deltaTime;
        }

        if (_timer >= _workTime)
        {
            _timer = 0;
            _workEnded = true;
            goToWarehouse();
        }
        
        //When unit get to resource it needs to gather
        if (transform.position.z == currentWorkPlace.transform.position.z && transform.position.y == currentWorkPlace.transform.position.y && !_gotToWorkPlace)
        {
            _gotToWorkPlace = true;
        }
        
        //When unit gets to working station to give resource to player
        if (transform.position.z == currentWarehouse.transform.position.z && transform.position.y == currentWarehouse.transform.position.y && _workEnded)
        {
            if (currentResource == ResourceTypeEnum.WOOD)
            {
                playerResources.Wood += amountResourceGather;
                currentWorkPlaceScript.ResourceAmount -= amountResourceGather;
            } else if (currentResource == ResourceTypeEnum.FOOD)
            {
                playerResources.Food += amountResourceGather;
                currentWorkPlaceScript.ResourceAmount -= amountResourceGather;
            } else
            {
                playerResources.Gold += amountResourceGather;
                currentWorkPlaceScript.ResourceAmount -= amountResourceGather;
            }

            if (currentWorkPlaceScript.ResourceAmount <= 0)
            {
                Destroy(currentWorkPlace);
            }
            _workEnded = false;
            _gotToWorkPlace = false;
            goToWorkPlace();
        }
    }
    
    //Change type of gathering resource
    public void ChangeResource(ResourceTypeEnum resource)
    {
        currentResource = resource;
    }
    
    //Change working station
    public void ChangeWarehouse(GameObject warehouse)
    {
        currentWarehouse = warehouse;
        //if(currentWarehouse != Mainbuilding)
        currentWarehouseScript = warehouse.GetComponent<PassiveBuilding>();
    }

    //Change Working place
    public void ChangeWorkPlace(GameObject workPlace)
    {
        currentWorkPlace = workPlace;
        currentWorkPlaceScript = workPlace.GetComponent<ResourceBuilding>();
        currentWorkPlaceScript.attachedUnit = gameObject;
        goToWorkPlace();
    }
    
    //Change working time
    public void ChangeWorkTime(float time)
    {
        _workTime = time;
    }

    //Change amount of resources unit can gather from resource
    public void ChangeAmountResourceGather(uint amount)
    {
        amountResourceGather = amount;
    }
    //unit go to work place
    private void goToWorkPlace()
    {
        //GoToPoint(currentWorkPlace.transform.position);
    }
    //Unit go to Working station
    private void goToWarehouse()
    {
        //GoToPoint(currentWarehouse.transform.position);
    }
    //public override void SetBuilding(ulong id)
    //{
    //}
}
