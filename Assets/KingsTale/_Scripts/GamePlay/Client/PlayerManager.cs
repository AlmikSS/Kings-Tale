using TMPro;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(UnitClick), typeof(UnitEnlight), typeof(UnitSelections))]
public class PlayerManager : NetworkBehaviour
{
    [SerializeField] private GameObject _ui;
    [SerializeField] private GameObject _camera;
    [SerializeField] private TMP_Text _resourcesText;
    
    public ResourcesStruct Resources { get; private set; }
    
    private UnitClick _unitClick;
    private UnitEnlight _unitEnlight;
    private UnitSelections _unitSelections;

    public override void OnNetworkSpawn()
    {
        if (!IsLocalPlayer)
        {
            _ui.SetActive(false);
            _camera.SetActive(false);
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
    }

    private void Update()
    {
        if (!IsLocalPlayer) return;
        
        if (Input.GetKeyDown(KeyCode.H))
        {
            var request = new ServerAddResourcesRequestStruct(new ResourcesStruct((uint)Random.Range(10, 20), (uint)Random.Range(10, 20), (uint)Random.Range(10, 20)), OwnerClientId);
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
            _unitSelections.unitList.Add(unit.gameObject);
        }

        _resourcesText.text = $"Wood: {Resources.Wood}, Food: {Resources.Food}, Gold: {Resources.Gold}";
    }
}