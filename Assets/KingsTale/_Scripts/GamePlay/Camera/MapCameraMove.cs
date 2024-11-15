using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Camera))]
public class MapCameraMove : MonoBehaviour
{
    [Header("CamSettings")]
    [SerializeField] [Range(0, 100)] private float DefaultSwipe;
    [SerializeField] [Range(0, 1)] private float CamSpeed;
    [SerializeField] [Range(1, 100)] private float YLevel = 8;
    [SerializeField] private InputActionReference _zoomRef;
    
    [Header("MapSettings")]
    public float MapSize = 10f;
    public float MapScaler = 0.00522f;
    
    [Header("Objects")]
    [SerializeField] private Transform _lBorder;
    [SerializeField] private Transform _rBorder;
    [SerializeField] private Transform _uBorder;
    [SerializeField] private Transform _dBorder;
    [SerializeField] private Transform _screenUp, _screenDown;
    [SerializeField] private Slider _zoomSlider;
    public RectTransform RectTF;
    
    private float _lGrBorder => _lBorder.position.x;
    private float _rGrBorder => _rBorder.position.x;
    private float _uGrBorder => _uBorder.position.z;
    private float _dGrBorder => _dBorder.position.z;
    
    private Vector2 _initialPos, _currentPos, _rectDirection, _startSize;
    private Vector3 _direction, _camInitial, _rectInitial, _camPrev;
    private Camera cam;


    public List<Transform> Lines = new(); //список всех линий
    public float ZoomScale { set; get; } = 25f;
    public bool CanMove { set; get; } = true;
    public bool EnableMove { set; get; } = true;
    public Vector3 CamInitial => _camInitial;
    private void Awake()
    {
        _currentPos = transform.position;
        _initialPos = _currentPos;

        _camInitial = transform.position;
        _direction = _camInitial;

        _startSize = RectTF.sizeDelta;
        cam = GetComponent<Camera>();
        ChangeZoom();
    }

    private void DetectSwipe()
    {
        Vector3 delta = _initialPos - _currentPos; // это разница в позициях мыши
        var factor = DefaultSwipe / 50 * ZoomScale; //это множитель со свайпом и зумом

        _direction.x =
            _camPrev.x +
            delta.x * MapScaler * MapSize / 50 * ZoomScale * factor; //это направление камеры по x вмировых кордах
        _direction.z =
            _camPrev.z +
            delta.y * MapScaler * MapSize / 50 * ZoomScale * factor; //это направление камеры по z вмировых кордах
    }

    public void ChangeZoom()
    {
        cam.orthographicSize = ZoomScale; //зум камеры
        RectTF.localScale = new Vector3(50 / ZoomScale, 50 / ZoomScale, 1); //зум ректа

        CheckBorders(); //проверка не зашел ли за край экрана
    }

    private void Update()
    {
        if (!CanMove || !EnableMove) return;
        
        if (Mathf.Abs(_zoomRef.action.ReadValue<Vector2>().y) > 0)
        {
            ZoomScale -= _zoomRef.action.ReadValue<Vector2>().y / 60f;
            ZoomScale = Mathf.Clamp(ZoomScale, 10, 50);

            _zoomSlider.value = ZoomScale;
            ChangeZoom();
        }

        _currentPos = UnityEngine.Input.mousePosition;
        if (UnityEngine.Input.GetMouseButtonDown(0))
        {
            _initialPos = _currentPos;
            _camPrev = transform.position;
            _rectInitial = RectTF.localPosition;
            _direction = _camPrev;
        }
        else if (UnityEngine.Input.GetMouseButton(0))
        {
            DetectSwipe();
        }
        else if (UnityEngine.Input.GetMouseButtonUp(0))
        {
            CheckBorders();
        }
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, _direction, CamSpeed);
        transform.position = new Vector3(transform.position.x, YLevel, transform.position.z);

        RectTF.localPosition = Vector3.Lerp(RectTF.localPosition,
            new Vector3(
                (-transform.position.x) / (MapScaler * MapSize / 50 * ZoomScale) +
                _camInitial.x / (MapScaler * MapSize / 50 * ZoomScale),
                (-transform.position.z) / (MapScaler * MapSize / 50 * ZoomScale) +
                _camInitial.z / (MapScaler * MapSize / 50 * ZoomScale), 0),
            CamSpeed);
    }


    private void CheckBorders()
    {
        var leftBorder = transform.position.x - cam.orthographicSize;
        var rightBorder = transform.position.x + cam.orthographicSize;
        var upBorder = transform.position.z + (cam.orthographicSize * Screen.height / Screen.width);
        var downBorder = transform.position.z - (cam.orthographicSize * Screen.height / Screen.width);

        if (leftBorder < _lGrBorder)
        {
            _direction.x = _lGrBorder + cam.orthographicSize;
            _rectDirection.x = (RectTF.sizeDelta.x * RectTF.localScale.x - _startSize.x) / 2;
        }

        if (rightBorder > _rGrBorder)
        {
            _direction.x = _rGrBorder - cam.orthographicSize;
            _rectDirection.x = (_startSize.x - RectTF.sizeDelta.x * RectTF.localScale.x) / 2;
        }

        if (upBorder > _uGrBorder)
        {
            _direction.z = _uGrBorder - (cam.orthographicSize * Screen.height / Screen.width);
            _rectDirection.y = (_startSize.y - RectTF.sizeDelta.y * RectTF.localScale.y) / 2 -
                               (_startSize.y / 2 - Vector2.Distance(_screenUp.localPosition, Vector2.zero));
        }

        if (downBorder < _dGrBorder)
        {
            _direction.z = _dGrBorder + (cam.orthographicSize * Screen.height / Screen.width);
            _rectDirection.y = (RectTF.sizeDelta.y * RectTF.localScale.y - _startSize.y) / 2 +
                               (_startSize.y / 2 - Vector2.Distance(_screenDown.localPosition, Vector2.zero));
        }
    }
}