using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class MouseOverUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private MainInput _controller;
    private bool _clicked;
    private CameraMovement _cam;

    public void Awake()
    {
        _cam = GameObject.FindWithTag("MainCamera").GetComponent<CameraMovement>();
    }
    
    public void OnDisable()
    {
        if (!_clicked) return;
        if (gameObject.name.StartsWith("Border")) transform.parent.GetComponent<BorderSwipes>().border = "";
        _cam.IsOverUI = false;
        _clicked = false;
    }

    public void OnPointerUp(PointerEventData eventData)
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