using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(UnitClick), typeof(UnitEnlight), typeof(UnitSelections))]
public class PlayerManager : NetworkBehaviour
{
    [SerializeField] private GameObject _ui;
    [SerializeField] private GameObject _camera;
    
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
}