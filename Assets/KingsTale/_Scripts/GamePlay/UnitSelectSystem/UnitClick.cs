using UnityEngine;
using UnityEngine.InputSystem;

public class UnitClick : MonoBehaviour
{
    private Maininput _input;
    [SerializeField] private Camera _myCam;

    public LayerMask clickable;
    public LayerMask ground;
    public LayerMask building;

    [SerializeField] private Vector3 pos;
    [SerializeField] private UnitSelections _unitSelections;

    private GameObject build;

    private void Awake()
    {
        _input = new Maininput();
    }

    private void OnEnable()
    {
        _input.Enable();
        _input.Player.RightClick.performed += OnUnitClick;
    }

    private void OnDisable()
    {
        _input.Disable();
        _input.Player.RightClick.performed -= OnUnitClick;
    }

    private void OnUnitClick(InputAction.CallbackContext context)
    {
        RaycastHit hit;
        Ray ray = _myCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, clickable))
        {
            _unitSelections.SelectUnit(hit.collider.gameObject);
        }
        else if(_unitSelections.unitSelected.Count != 0 && Physics.Raycast(ray, out hit, Mathf.Infinity, ground))
        {
            pos = hit.point;
            //Request (playerid, pos, UnitSelections.Instance.unitSelected) can we move here or not (response send to units)
            _unitSelections.Deselect();
        } else if (_unitSelections.unitSelected.Count != 0 &&
                   Physics.Raycast(ray, out hit, Mathf.Infinity, building))
        {
            //Request (playerid, buildid, UnitSelections.Instance.unitsSelected) to get can we send units here or not (response send to units)
            build = hit.collider.gameObject;
        }
    }
}
