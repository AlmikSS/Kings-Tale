using UnityEngine;

public class ResourceBuilding : Building
{
    [SerializeField] private ResourceTypeEnum resourceType;
    public uint ResourceAmount;
    public GameObject attachedUnit;
    public ResourceTypeEnum GetResourceType()
    {
        return resourceType;
    }
}
