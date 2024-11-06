using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingSystem : NetworkBehaviour
{
    [SerializeField] private GameObject _unitControlSystem;
    [SerializeField] private Camera _myCam;
    [SerializeField] private List<Building> _buildings = new();
    
    private Maininput _maininput;
    private GameObject _currBuildPrefab;
    private Building _buildPrefabScript;
    public LayerMask ground;

    private void Awake()
    {
        _maininput = new Maininput();
    }

    private void OnEnable()
    {
        _maininput.Enable();
        _maininput.Player.MoveMouse.performed += MoveHologram;
        _maininput.Player.LeftClick.performed += PlaceBuilding;
    }

    private void OnDisable()
    {
        _maininput.Disable();
        _maininput.Player.MoveMouse.performed -= MoveHologram;
        _maininput.Player.LeftClick.performed -= PlaceBuilding;
    }

    [Rpc(SendTo.Owner)]
    public void StartPlacingBuildingRpc(ushort id)
    {
        Ray ray = _myCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        var buildPrefab = GetBuilding(id);
        
        Physics.Raycast(ray, out hit, Mathf.Infinity, ground);
        _currBuildPrefab = Instantiate(buildPrefab, hit.point, Quaternion.identity).GetComponent<GameObject>();
        _buildPrefabScript = _currBuildPrefab.GetComponent<Building>();
        _unitControlSystem.SetActive(false);
    }

    private Building GetBuilding(ushort id)
    {
        foreach (var building in _buildings)
        {
            if (building.Id == id)
                return building;
        }

        return null;
    }
    
    private void MoveHologram(InputAction.CallbackContext ctx)
    {
        if (_currBuildPrefab != null)
        {
            Ray ray = _myCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
                    
            Physics.Raycast(ray, out hit, Mathf.Infinity, ground);
            _currBuildPrefab.transform.position = hit.point;
        }

    }

    private void PlaceBuilding(InputAction.CallbackContext ctx)
    {
        if (_currBuildPrefab != null && _buildPrefabScript.CanBuild)
        {
            Ray ray = _myCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, Mathf.Infinity, ground);
            
            var request = new ServerPlaceBuildingRequestStruct();
            request.BuildingId = _currBuildPrefab.GetComponent<Building>().Id;
            request.PlayerId = NetworkObject.OwnerClientId;
            request.Position = hit.point;
            InputManager.Instance.HandlePlaceBuildingRequestRpc(request);
        }
    }

    [Rpc(SendTo.Owner)]
    public void OnBuildingPlacedRpc()
    {
        _unitControlSystem.SetActive(true);
        Destroy(_currBuildPrefab);
        _currBuildPrefab = null;
    }
}
