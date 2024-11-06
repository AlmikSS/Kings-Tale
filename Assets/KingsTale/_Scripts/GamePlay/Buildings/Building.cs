using Unity.Netcode;
using UnityEngine;

public class Building : NetworkBehaviour
{
    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField] private float _buildHealth;
    [SerializeField] private BuildType _buildType;
    [SerializeField] private ushort _id;

    public bool CanBuild { get; private set; } = true;
    
    public ushort Id => _id;
    
    public float GetBuildHealth()
    {
        return _buildHealth;
    }

    public BuildType GetBuildType()
    {
        return _buildType;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == _obstacleMask)
            CanBuild = false;
    }
    
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == _obstacleMask)
            CanBuild = true;
    }
}

public enum BuildType
{
    ATTACK, PASSIVE
}

