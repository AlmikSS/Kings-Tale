using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Buildings/AFKBuilding", fileName = "NewAFKBuildingConfig")]
public class AFKBuildingConfigSO : BuildingBaseConfigSO
{
    [Header("Options")]
    [SerializeField] private ResourcesStruct _resourcesToAdd;
    [SerializeField] private float _culDown;

    public ResourcesStruct ResourcesToAdd => _resourcesToAdd;

    public float CulDown => _culDown;

    public void SetProperties(uint maxHealth, uint magicResist, uint physicalResist, ResourcesStruct price, float buildTime, ResourcesStruct resourcesToAdd, float culDown)
    {
        base.SetProperties(maxHealth, magicResist, physicalResist, price, buildTime);

        _resourcesToAdd = resourcesToAdd;
        _culDown = culDown;
    }
}