using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Buildings/WorkingBuilding", fileName = "NewWorkingBuildingConfig")]
public class WorkingBuildingConfigSO : BuildingBaseConfigSO
{
    [Header("Options")]
    [SerializeField] private ResourcesStruct _resourcesToAdd;
    [SerializeField] private float _workTime;
    [SerializeField] private float _workRange;
    [SerializeField] private float _restTime;
    [SerializeField] private uint _maxWorkersCount;

    public ResourcesStruct ResourcesToAdd => _resourcesToAdd;

    public float WorkTime => _workTime;

    public float WorkRange => _workRange;

    public float RestTime => _restTime;

    public uint MaxWorkersCount => _maxWorkersCount;

    public void SetProperties(uint maxHealth, uint magicResist, uint physicalResist, ResourcesStruct price, float buildTime, ResourcesStruct resourcesToAdd, float workTime, float workRange, float restTime, uint maxWorkersCount)
    {
        base.SetProperties(maxHealth, magicResist, physicalResist, price, buildTime);

        _resourcesToAdd = resourcesToAdd;
        _workTime = workTime;
        _workRange = workRange;
        _restTime = restTime;
        _maxWorkersCount = maxWorkersCount;
    }
}