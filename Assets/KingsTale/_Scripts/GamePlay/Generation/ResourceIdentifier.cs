using UnityEngine;

public enum ResourceType
{
    Gold,
    CentralGold,
    Tree,
    Stone
}

public class ResourceIdentifier : MonoBehaviour
{
    public ResourceType resourceType;
}
