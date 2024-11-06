using System;
using UnityEngine;

public class BuildHologram : MonoBehaviour
{
    public GameObject buildActual;
    public bool canBuild {get; private set;}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 7)
        {
            canBuild = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == 7)
        {
            canBuild = true;
        }
    }
}
