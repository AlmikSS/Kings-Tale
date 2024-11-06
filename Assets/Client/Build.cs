using System.Collections;
using UnityEngine;

public class Build : MonoBehaviour
{
    [SerializeField] private float buildHealth;
    [SerializeField] private BuildType buildType;
    public float GetBuildHealth()
    {
        return buildHealth;
    }

    public BuildType GetBuildType()
    {
        return buildType;
    }
}

public enum BuildType
{
    ATTACK, PASSIVE
}

