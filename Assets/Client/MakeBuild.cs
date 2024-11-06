using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MakeBuild : MonoBehaviour
{
    [SerializeField] private GameObject UnitControlSystem;
    private Camera _myCam;
    private Maininput _maininput;
    private GameObject _currBuildPrefab;
    private BuildHologram _buildPrefabScript;
    public LayerMask ground;

    private void Awake()
    {
        _maininput = new Maininput();
    }

    private void OnEnable()
    {
        _maininput.Enable();
        _maininput.Player.MoveMouse.performed += MoveHologram;
        _maininput.Player.LeftClick.performed += MakeBuilding;
    }

    private void OnDisable()
    {
        _maininput.Disable();
        _maininput.Player.MoveMouse.performed -= MoveHologram;
        _maininput.Player.LeftClick.performed -= MakeBuilding;
    }

    private void Start()
    {
        _myCam = Camera.main;
    }
    public void BuyBuild(GameObject buildPrefab)
    {
        Ray ray = _myCam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        Physics.Raycast(ray, out hit, Mathf.Infinity, ground);
        _currBuildPrefab = Instantiate(buildPrefab, hit.point, Quaternion.identity);
        _buildPrefabScript = _currBuildPrefab.GetComponent<BuildHologram>();
        UnitControlSystem.SetActive(false);
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

    private void MakeBuilding(InputAction.CallbackContext ctx)
    {
        if (_currBuildPrefab != null)
        {
            Ray ray = _myCam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
                    
            Physics.Raycast(ray, out hit, Mathf.Infinity, ground);
            Instantiate(_buildPrefabScript.buildActual, hit.point, Quaternion.identity);
            UnitControlSystem.SetActive(true);
            Destroy(_currBuildPrefab);
            _currBuildPrefab = null;
        }
    }
}
