using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(UnitClick), typeof(UnitEnlight), typeof(UnitSelections))]
public class PlayerManager : NetworkBehaviour
{
    [SerializeField] private LayerMask _interactLayerMask;
    [SerializeField] private float _seekTreeRange;
    private GameObject _ui;
    private Camera _camera;
    private TMP_Text _resourcesText;

    public ResourcesStruct Resources { get; private set; }
    public MainBuilding MainBuilding { get; private set; }
    public UnitSelections UnitSelections => _unitSelections;

    private UnitClick _unitClick;
    private UnitEnlight _unitEnlight;
    private UnitSelections _unitSelections;
    private BuildingSystem _buildingSystem;

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
        _buildingSystem = GetComponent<BuildingSystem>();

        _input = FindFirstObjectByType<PlayerInput>();
        _input.actions["LeftClick"].performed += OnMouseLeftClick;
        _input.actions["RightClick"].performed += OnMouseRightClick;
        
        _camera = GameObject.FindWithTag("MainCam").GetComponent<Camera>();
        _ui = GameObject.FindWithTag("UI");
        _resourcesText = GameObject.FindWithTag("Resources").GetComponent<TMP_Text>();
    }

    private void Update()
    {
        if (!IsLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.H))
        {
            var request = new ServerAddResourcesRequestStruct(
                new ResourcesStruct((uint)Random.Range(10, 20), (uint)Random.Range(10, 20), (uint)Random.Range(10, 20)),
                OwnerClientId);
            InputManager.Instance.HandleAddResourcesRequestRpc(request);
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

        _resourcesText.text = $"Дерево: {Resources.Wood}, Еда: {Resources.Food}, Золото: {Resources.Gold}";
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

        var pointerPos = _input.actions["PointerPos"].ReadValue<Vector2>();
        var ray = _camera.ScreenPointToRay(pointerPos);
        Physics.Raycast(ray, out var hit, _interactLayerMask);

        if (hit.collider.gameObject.TryGetComponent(out MainBuilding mainBuilding))
        {
            if (mainBuilding.IsBuilt)
                mainBuilding.BuyWorkerUnit();
        }
        
        if (_unitSelections.unitSelected.Count > 0)
        {
            if (hit.collider.gameObject.CompareTag("Ground") && !_buildingSystem.IsBuilding)
                SetDestination(hit.point);
            else if (hit.collider.gameObject.TryGetComponent(out Building building))
            {
                if (!building.IsBuilt)
                {
                    SetUnitBuilding(_unitSelections.unitSelected[0].NetworkObjectId, hit.collider.gameObject.GetComponent<NetworkObject>().NetworkObjectId);
                    _unitSelections.Deselect();
                    return;
                }

                if (building is Tree && _unitSelections.unitSelected.Count > 1)
                {
                    var cols = Physics.OverlapSphere(hit.point, _seekTreeRange);
                    int i = 0;

                    foreach (var col in cols)
                    {
                        if (col.gameObject.TryGetComponent(out Tree tree))
                        {
                            i++;
                            SetUnitBuilding(_unitSelections.unitSelected[i].NetworkObjectId, tree.NetworkObjectId, false);
                        }
                        if (i > _unitSelections.unitSelected.Count)
                            break;
                    }
                    
                    _unitSelections.Deselect();
                }
                else
                {
                    foreach (var unit in _unitSelections.unitSelected)
                        SetUnitBuilding(unit.NetworkObjectId, hit.collider.gameObject.GetComponent<NetworkObject>().NetworkObjectId);
                    _unitSelections.Deselect();
                }
            }
        }
    }

    private void SetUnitBuilding(ulong unitId, ulong buildingId, bool isOwned = true)
    {
        var request = new ServerSetUnitBuildingRequestStruct
        {
            PlayerId = OwnerClientId,
            UnitId = unitId,
            BuildingId = buildingId,
            IsOwned = isOwned
        };

        InputManager.Instance.HandleSetUnitBuildingRequestRpc(request);
    }
    
    private void SetDestination(Vector3 point)
    {
        foreach (var unit in _unitSelections.unitSelected)
        {
            var request = new ServerSetUnitDestinationRequestStruct
            {
                PlayerId = OwnerClientId,
                UnitId = unit.GetComponent<NetworkObject>().NetworkObjectId,
                Point = point
            };

            InputManager.Instance.HandleSetUnitDestinationRequestRpc(request);
        }
    }

    private void OnMouseRightClick(InputAction.CallbackContext obj)
    {
        if (!IsLocalPlayer) return;

        _unitSelections.Deselect();
    }
}