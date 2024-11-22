using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class UnitEnlight : MonoBehaviour
{
    private PlayerInput _mainInput;
    private Camera _myCam;
    private bool _click = false;

    private RectTransform boxVisual;
    [SerializeField] private UnitSelections _unitSelections;

    private Rect _selectionBox;
    private Vector2 _startPos;
    private Vector2 _endPos;

    void Awake()
    {
        _myCam = GameObject.FindWithTag("MainCam").GetComponent<Camera>();
        boxVisual = GameObject.FindWithTag("BoxVisual").GetComponent<RectTransform>();
        _mainInput = FindFirstObjectByType<PlayerInput>();
        _mainInput.actions[GamePlayConstants.ACT_LEFT_CLICK].performed += StartVisual;
        _mainInput.actions[GamePlayConstants.ACT_LEFT_CLICK].canceled += EndVisual;
    }
    void Start()
    {
        _startPos= Vector2.zero;
        _endPos= Vector2.zero;
        DrawVisual();
    }

    private void Update()
    {
        if (_click)
        {
            _endPos = Input.mousePosition;
            DrawVisual();
            DrawSelection();
        }
    }

    private void StartVisual(InputAction.CallbackContext ctx)
    {
        _startPos = Input.mousePosition;
        _click = true;
    }
    private void DrawVisual()
    {
        Vector2 boxStart = _startPos;
        Vector2 boxEnd = _endPos;
        
        Vector2 boxCenter = (boxStart + boxEnd) / 2f;
        boxVisual.position = boxCenter;
        
        Vector2 boxSize = new Vector2(Mathf.Abs(boxEnd.x - boxStart.x), Mathf.Abs(boxEnd.y - boxStart.y));
        
        boxVisual.sizeDelta = boxSize;
    }

    private void EndVisual(InputAction.CallbackContext ctx)
    {
        SelectUnits();
        _endPos = Vector2.zero;
        _startPos = Vector2.zero;
        _click = false;
        DrawVisual();
    }

    private void DrawSelection()
    {
        if (Input.mousePosition.x < _startPos.x)
        {
            _selectionBox.xMin = Input.mousePosition.x;
            _selectionBox.xMax = _startPos.x;
        }
        else
        {
            _selectionBox.xMax = Input.mousePosition.x;
            _selectionBox.xMin = _startPos.x;
        }

        if (Input.mousePosition.y < _startPos.y)
        {
            _selectionBox.yMin = Input.mousePosition.y;
            _selectionBox.yMax = _startPos.y;
        }
        else
        {
            _selectionBox.yMax = Input.mousePosition.y;
            _selectionBox.yMin = _startPos.y;
        }
    }

    private void SelectUnits()
    {
        foreach (var unit in _unitSelections.unitList.Where(unit => _selectionBox.Contains(_myCam.WorldToScreenPoint(unit.transform.position))))
        {
            if (unit.TryGetComponent(out UnitBrain unitBrain))
                _unitSelections.DragSelect(unitBrain);
        }
    }
}
