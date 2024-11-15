using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(Camera))]
public class CameraMovement : NetworkBehaviour
{
	[Range(0, 20)][SerializeField] private float _camSpeed;
	[Range(0, 20)][SerializeField] private float _camSpeedForTouch;
	[Range(0, 2f)][SerializeField] private float _smoothTime;
	[Range(0, 2f)][SerializeField] private float _smoothTimeForTouch;
	[SerializeField] private PlayerInput _controller;
	[SerializeField] private LayerMask _nonMovebleMask;
	[SerializeField] private GameObject _mapCam;
	[SerializeField] private Slider _zoomSlider;
	
	private Vector2 _clickedPos;
	private Vector3 _direction;
	private Camera _cam;
	private bool _mouseMove;
	private Vector3 _velocity = Vector3.zero;
	
	public bool IsOverUI { set; get; }
	public float ZoomScale { set; get; } = 50f;
	public bool CanMove { set; get; } = true;
	
	private void Awake()
	{
		_cam = GetComponent<Camera>();
	}

	private void OnEnable()
	{
		_mapCam.SetActive(false);
		_controller = FindFirstObjectByType<PlayerInput>();
		_controller.actions["LeftClick"].started += Click;
		_controller.actions["MiddleClick"].performed += MouseMove;
		_controller.actions["MiddleClick"].canceled += MouseMoveOver;
	}

	private void OnDisable()
	{
		_mapCam.SetActive(true);
		_controller.actions["LeftClick"].started -= Click;
		_controller.actions["MiddleClick"].performed -= MouseMove;
		_controller.actions["MiddleClick"].canceled -= MouseMoveOver;
	}

	public void Update()
	{
		var lastScale = ZoomScale;
		//if (!IsOwner) { return; }
		if (_zoomSlider.IsActive())
		{
			ZoomScale -= _controller.actions["Zoom"].ReadValue<Vector2>().y / 60f;
			ZoomScale = Mathf.Clamp(ZoomScale, 20, 90);
			_zoomSlider.value = ZoomScale;
			_cam.fieldOfView = ZoomScale;
			
		}
		
		if (!CanMove) return;
		var realSpeed = _camSpeed * ZoomScale / 35;
		var realSpeedForTouch = _camSpeedForTouch * ZoomScale / 35;

		var actionVector = _mouseMove ? 
			_controller.actions["MoveMouse"].ReadValue<Vector2>() : 
			_controller.actions["MoveCamera"].ReadValue<Vector2>();
		if (PlayerPrefs.GetInt("InvertMouse") == 1 && _mouseMove)
			actionVector = new Vector2(-actionVector.x, -actionVector.y);
		_direction = transform.localPosition + new Vector3(actionVector.x, 0, actionVector.y);
		#if UNITY_ANDROID && !UNITY_EDITOR
		transform.localPosition = Vector3.SmoothDamp(transform.localPosition, _direction, ref _velocity, _smoothTimeForTouch,realSpeedForTouch);
		#else
		transform.localPosition = Vector3.SmoothDamp(transform.localPosition, _direction, ref _velocity, _mouseMove ? _smoothTimeForTouch : _smoothTime, _mouseMove ? realSpeedForTouch : realSpeed);
		#endif
		transform.position = new Vector3(Mathf.Clamp(transform.position.x, -40,46),transform.position.y,Mathf.Clamp(transform.position.z, -36f,48));
	}

	private void Click(InputAction.CallbackContext _)
	{
		_clickedPos = _controller.actions["PointerPos"].ReadValue<Vector2>();
		#if UNITY_ANDROID && !UNITY_EDITOR
		var ray = _cam.ScreenPointToRay(_clickedPos);
		if (Physics.Raycast(ray, out _, 100, _nonMovebleMask))
			CanMove = false;
		else
			CanMove = true;
		#endif
	}

	private void MouseMove(InputAction.CallbackContext _)
	{
		_mouseMove = true;
		GameplayButtons.TransparentUI();
	}

	private void MouseMoveOver(InputAction.CallbackContext _)
	{
		_mouseMove = false;
		GameplayButtons.NotTransparentUI();
	}
}
