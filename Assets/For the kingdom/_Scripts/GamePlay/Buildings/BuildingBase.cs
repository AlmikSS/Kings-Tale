using System;
using Unity.Netcode;
using UnityEngine;

public abstract class BuildingBase : NetworkBehaviour
{
    public bool CanPlace { get; private set; }
    
    private bool _isPlaced;

    private void OnTriggerStay(Collider other)
    {
        if (_isPlaced) return;

        if (other.CompareTag(GamePlayConstants.BUILDING_TAG))
            CanPlace = false;
    }

    private void OnTriggerExit(Collider other)
    {
        if (_isPlaced) return;

        if (other.CompareTag(GamePlayConstants.BUILDING_TAG))
            CanPlace = true;
    }
}