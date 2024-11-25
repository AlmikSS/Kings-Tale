using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Buildings/MainBuilding", fileName = "NewMainBuildingConfig")]
public class MainBuildingConfigSO : BuildingBaseConfigSO
{
    [Header("Options")]
    [SerializeField] private ResourcesStruct _resourcesToAdd;
    [SerializeField] private ushort _workerUnitId;
    [SerializeField] private float _culDown;

    public ResourcesStruct ResourcesToAdd => _resourcesToAdd;

    public ushort WorkerUnitId => _workerUnitId;

    public float CulDown => _culDown;

    public void SetProperties(uint maxHealth, uint magicResist, uint physicalResist, ResourcesStruct price, float buildTime, ResourcesStruct resourcesToAdd, ushort workerUnitId, float culDown)
    {
        base.SetProperties(maxHealth, magicResist, physicalResist, price, buildTime);

        _resourcesToAdd = resourcesToAdd;
        _workerUnitId = workerUnitId;
        _culDown = culDown;
    }
}