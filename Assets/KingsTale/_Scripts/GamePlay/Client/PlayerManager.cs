using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(UnitClick), typeof(UnitEnlight), typeof(UnitSelections))]
public class PlayerManager : NetworkBehaviour
{
    [SerializeField] private GameObject _ui;
    [SerializeField] private Camera _camera;
    [SerializeField] private TMP_Text _resourcesText;
    [SerializeField] private LayerMask _interactLayerMask;
    
    public ResourcesStruct Resources { get; private set; }
    public MainBuilding MainBuilding { get; private set; }
    
    private UnitClick _unitClick;
    private UnitEnlight _unitEnlight;
    private UnitSelections _unitSelections;

    private PlayerInput _input;
    
    public override void OnNetworkSpawn()
    {
        if (!IsLocalPlayer)
        {
            _ui.SetActive(false);
            _camera.gameObject.SetActive(false);
            GetComponent<BuildingSystem>().enabled = false;
            GetComponent<UnitClick>().enabled = false;
            GetComponent<UnitEnlight>().enabled = false;
            GetComponent<UnitSelections>().enabled = false;
            enabled = false;
            return;
        }

        _unitClick = GetComponent<UnitClick>();
        _unitEnlight = GetComponent<UnitEnlight>();
        _unitSelections = GetComponent<UnitSelections>();

        _input = FindFirstObjectByType<PlayerInput>();
        _input.actions["LeftClick"].performed += OnMouseLeftClick;
        _input.actions["RightClick"].performed += OnMouseRightClick;
    }

    private void Update()
    {
        if (!IsLocalPlayer) return;
        
        if (Input.GetKeyDown(KeyCode.H))
        {
            var request = new ServerAddResourcesRequestStruct(new ResourcesStruct((uint)Random.Range(10, 20), (uint)Random.Range(10, 20), (uint)Random.Range(10, 20)), OwnerClientId);
            InputManager.Instance.HandleAddResourcesRequestRpc(request);
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            var request = new ServerBuyRequestStruct
            {
                IsBuilding = false,
                PlayerId = OwnerClientId,
                Id = 0,
                Position = new Vector3()
            };
        
            InputManager.Instance.HandleBuyRequestRpc(request);
        }
    }
    
    [Rpc(SendTo.Owner)]
    public void UpdateStateRpc(ClientUpdateStateStruct updateState)
    {
        Debug.Log("Updated state. Client: " + OwnerClientId);

        Resources = updateState.Resources;
        
        _unitSelections.unitList.Clear();
        
        foreach (var unitId in updateState.Units)
        {
            var unit = NetworkManager.SpawnManager.SpawnedObjects[unitId];
            _unitSelections.unitList.Add(unit.GetComponent<UnitBrain>());
        }

        _resourcesText.text = $"Wood: {Resources.Wood}, Food: {Resources.Food}, Gold: {Resources.Gold}";
    }

    [Rpc(SendTo.Owner)]
    public void SetMainBuildingRpc(NetworkObjectReference mainBuilding)
    {
        if (mainBuilding.TryGet(out NetworkObject networkObject))
        {
            MainBuilding = networkObject.GetComponent<MainBuilding>();
        }
    }
    
    private void OnMouseLeftClick(InputAction.CallbackContext obj)
    {
        if (!IsLocalPlayer) return;
        
        if (_unitSelections.unitSelected.Count > 0)
        {
            var pointerPos = _input.actions["PointerPos"].ReadValue<Vector2>();
            var ray = _camera.ScreenPointToRay(pointerPos);
            RaycastHit hit;
            Physics.Raycast(ray, out hit, _interactLayerMask);
            
            if (hit.collider.gameObject.CompareTag("Ground"))
            {
                foreach (var unit in _unitSelections.unitSelected)
                {
                    var request = new ServerSetUnitDestinationRequestStruct
                    {
                        PlayerId = OwnerClientId,
                        UnitId = unit.GetComponent<NetworkObject>().NetworkObjectId,
                        Point = hit.point
                    };

                    InputManager.Instance.HandleSetUnitDestinationRequestRpc(request);
                }
            }
            else if (hit.collider.gameObject.CompareTag("Building"))
            {
                foreach (var unit in _unitSelections.unitSelected)
                {
                    var request = new ServerSetUnitBuildingRequestStruct
                    {
                        PlayerId = OwnerClientId,
                        UnitId = unit.NetworkObjectId,
                        BuildingId = hit.collider.gameObject.GetComponent<NetworkObject>().NetworkObjectId
                    };
                    
                    InputManager.Instance.HandleSetUnitBuildingRequestRpc(request);
                }
                
                _unitSelections.Deselect();
            }
        }
    }

    private void OnMouseRightClick(InputAction.CallbackContext obj)
    {
        if (!IsLocalPlayer) return;

        _unitSelections.Deselect();
    }
}