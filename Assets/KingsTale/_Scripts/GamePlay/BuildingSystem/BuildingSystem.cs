using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class BuildingSystem : NetworkBehaviour
{
    [SerializeField] private List<Building> _buildings = new();
    private GameObject _buildingShopCanvas, _gamePlayCanvas,_unitControlSystem;
    private Camera _myCam;

    public bool IsBuilding { get; private set; }
    
    private PlayerManager _playerManager;
    private PlayerInput _maininput;
    private GameObject _currBuildPrefab;
    private Building _buildPrefabScript;
    public LayerMask ground;

    public override void OnNetworkSpawn()
    {
        _playerManager = GetComponent<PlayerManager>();
        
        _myCam = GameObject.FindWithTag("MainCam").GetComponent<Camera>();
        _buildingShopCanvas = GameObject.FindWithTag("ShopCanvas").transform.GetChild(0).gameObject;
        _gamePlayCanvas = GameObject.FindWithTag("GameplayCanvas");
        _unitControlSystem = GameObject.FindWithTag("VisualCanvas");
        _maininput = FindFirstObjectByType<PlayerInput>();
        _maininput.actions["LeftClick"].performed += PlaceBuilding;
        _maininput.actions["MoveMouse"].performed += MoveHologram;
    }

    [Rpc(SendTo.Owner)]
    public void StartPlacingBuildingRpc(ushort id)
    {
        _buildingShopCanvas.SetActive(false);
        _gamePlayCanvas.SetActive(true);
        
        Ray ray = _myCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        var buildPrefab = GetBuilding(id);
        
        Physics.Raycast(ray, out hit, Mathf.Infinity, ground);
        _buildPrefabScript = Instantiate(buildPrefab, hit.point, Quaternion.identity);
        _currBuildPrefab = _buildPrefabScript.gameObject;
        _unitControlSystem.SetActive(false);

        IsBuilding = true;
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
            
            IsBuilding = false;
        }
    }

    [Rpc(SendTo.Owner)]
    public void OnBuildingPlacedRpc(ulong id)
    {
        _unitControlSystem.SetActive(true);
        Destroy(_currBuildPrefab);
        _currBuildPrefab = null;
        if (_playerManager.UnitSelections.unitSelected.Count > 0)
        {
            var request = new ServerSetUnitBuildingRequestStruct
            {
                PlayerId = OwnerClientId,
                BuildingId = id,
                UnitId = _playerManager.UnitSelections.unitSelected[0].NetworkObjectId
            };
            
            InputManager.Instance.HandleSetUnitBuildingRequestRpc(request);
            
            _playerManager.UnitSelections.Deselect();
        }
    }

    [Rpc(SendTo.Owner)]
    public void SetBuildingListRpc(ulong gameDataID)
    {
        var gameData = NetworkManager.Singleton.SpawnManager.SpawnedObjects[gameDataID].GetComponent<GameData>();
        
        foreach (var prefab in gameData.BuildingsPrefabs)
        {
            if (prefab.TryGetComponent(out Building building))
                _buildings.Add(building);
        }
    }
}
