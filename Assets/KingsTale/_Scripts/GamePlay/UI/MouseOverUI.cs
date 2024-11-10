using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MouseOverUI : MonoBehaviour, IPointerDownHandler
{
    private MainInput _controller;
    private bool _clicked;
    private CameraMovement _cam;

    public void Awake()
    {
        //_cam = FindObjectsOfType<PlayerManager>()[0].PlayerCam.GetComponent<CameraMovement>();
    }

    public void OnEnable()
    {
        _controller = new MainInput();
        _controller.Player.Enable();
        _controller.Player.LeftClick.started += Click;
    }
    
    public void OnDisable()
    {
        if (_clicked)
        {
            if (gameObject.name.StartsWith("Border")) transform.parent.GetComponent<BorderSwipes>().border = "";
            _cam.IsOverUI = false;
            _clicked = false;
        }
        _controller.Player.Disable();
        _controller.Player.LeftClick.started -= Click;
    }

    private void Click(InputAction.CallbackContext _)
    {
        if (!_clicked) return;
        if (gameObject.name.StartsWith("Border")) transform.parent.GetComponent<BorderSwipes>().border = "";
        _cam.IsOverUI = false;
        _clicked = false;
    }
    
    public void OnPointerDown(PointerEventData eventData)
    {
        if (gameObject.name.StartsWith("Border")) transform.parent.GetComponent<BorderSwipes>().border = gameObject.name;

        _cam.IsOverUI = true;
        _clicked = true;
    }
}