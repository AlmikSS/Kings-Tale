using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraControl : MonoBehaviour
{
    [SerializeField] private float speedRot = 1f, speed = 1f;
    private Maininput _maininput;
    private Camera _myCam;
    private Vector2 _mousePos;
    private bool _mouseDown = false;
    
    private Quaternion _mouseRot;

    private void Awake()
    {
        _maininput = new Maininput();
    }

    private void Start()
    {
        _myCam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            _myCam.transform.Translate(Vector3.up * speed * Time.deltaTime);
        } else if (Input.GetKey(KeyCode.S))
        {
            _myCam.transform.Translate(Vector3.down * speed * Time.deltaTime);
        } else if (Input.GetKey(KeyCode.A))
        {
            _myCam.transform.Translate(Vector3.left * speed * Time.deltaTime);
        } else if (Input.GetKey(KeyCode.D))
        {
            _myCam.transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
        if (_mouseDown)
        {
            CameraMove();
        }
    }

    private void OnEnable()
    {
        _maininput.Enable();
        _maininput.Player.MiddleClick.performed += StartMove;
        _maininput.Player.MiddleClick.canceled += EndMove;
    }

    private void OnDisable()
    {
        _maininput.Disable();
        _maininput.Player.MiddleClick.performed -= StartMove;
        _maininput.Player.MiddleClick.canceled -= EndMove;
    }

    private void StartMove(InputAction.CallbackContext ctx)
    {
        _mousePos = Input.mousePosition;
        _mouseRot = _myCam.transform.rotation;
        _mouseDown = true;
    }

    private void EndMove(InputAction.CallbackContext ctx)
    {
        _mouseDown = false;
    }

    private void CameraMove()
    {
        _myCam.gameObject.transform.rotation = Quaternion.Euler(_mouseRot.eulerAngles.x - (Input.mousePosition.y - _mousePos.y) / (1 / speedRot)
            ,_mouseRot.eulerAngles.y - (Input.mousePosition.x - _mousePos.x) / (1 / speedRot), 0);
    }
}
