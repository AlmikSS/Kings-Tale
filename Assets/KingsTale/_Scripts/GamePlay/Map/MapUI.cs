using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MapUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler//, IPointerEnterHandler, IPointerExitHandler
{

   	public bool DrawArrow {get; set;} = true;
    public bool CanDraw { get; set; }
    public bool DeleteNote { get; set; }
    
	[Header("Settings")]
	[SerializeField] private float _minDistance = 15f;
	[SerializeField] private float _lineWidth = 10f;

	[Header("Objects")]
    [SerializeField] private PlayerManager _pm;
    [SerializeField] private Camera _mapCam;
	[SerializeField] private Material _lineMaterial;
    [SerializeField] private GameObject _linePrefab, _lineCanvas, _lineNoArrow;
    

	[Header("Script")]
    [SerializeField] private MapCameraMove _mapZooming;
    
	private RectTransform _rectTransform => _mapZooming.RectTF;
    private RectTransform arrowRectTransform;
    private CanvasRenderer lineCanvasRenderer;
    private float _toWorldFactor;

    private Mesh mesh;
    private List<Bounds> bounds = new();
    private Vector2 lastLinePoint;

    

    private List<Vector3> _positions = new();
    private List<List<AttackUnit>> _movingGroups = new();
    private List<GameObject> _createdLines = new();
    private List<GameObject> _createdNotes = new();
    private GameObject _line;




    private void Awake() {
		_toWorldFactor = _mapZooming.MapScaler * _mapZooming.MapSize;
    }



    public void OnBeginDrag(PointerEventData eventData)
    {

        if(CanDraw /*&& _pm.PlayerUnits.UnitsSelected.Count > 0 && DrawArrow*/)
        {
            //create new line
            _line = Instantiate(_linePrefab, _lineCanvas.transform) as GameObject;
            lineCanvasRenderer = _line.GetComponent<CanvasRenderer>();
            
            
            var randColor = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
            Material _material = new Material(Shader.Find("UI/Default"));
            _material.color = randColor;
            lineCanvasRenderer.SetMaterial(_material, null);
            _line.transform.GetChild(0).GetComponent<Image>().color = randColor;
            
            arrowRectTransform = _line.transform.GetChild(0).GetComponent<RectTransform>();
            _mapZooming.Lines.Add(_line.transform);
            //create new mesh
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, Input.mousePosition,null, out Vector2 lastLinePoint);

            mesh = MeshUtils.CreateMesh(lastLinePoint, lastLinePoint, lastLinePoint, lastLinePoint);
            lineCanvasRenderer.SetMesh(mesh);
            arrowRectTransform.gameObject.SetActive(true);
            //add point to positions
            
            _positions.Add(new Vector3(lastLinePoint.x * _toWorldFactor + _mapZooming.CamInitial.x ,0,lastLinePoint.y * _toWorldFactor + _mapZooming.CamInitial.z));
            
            //add units in group and created line
            // if (!_movingGroups.Contains(_pm.PlayerUnits.UnitsSelected))
            // {
            //     _movingGroups.Add(new List<AttackUnit>(_pm.PlayerUnits.UnitsSelected));
            //     _createdLines.Add(_line);
            // }
            // else
            // {
            //     var ind = _movingGroups.IndexOf(_pm.PlayerUnits.UnitsSelected);
            //     Destroy(_createdLines[ind]);
            //     _createdLines[ind] = _line;
            // }
        }
        else if(CanDraw)
        {
	        _line = Instantiate(_lineNoArrow, _lineCanvas.transform) as GameObject;
	        lineCanvasRenderer = _line.GetComponent<CanvasRenderer>();
	        lineCanvasRenderer.SetMaterial(_lineMaterial, null);
	        _createdNotes.Add(_line);
	        
	        RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, Input.mousePosition,null, out Vector2 lastLinePoint);
	        mesh = MeshUtils.CreateMesh(lastLinePoint, lastLinePoint, lastLinePoint, lastLinePoint);
	        lineCanvasRenderer.SetMesh(mesh);
	        
	        bounds.Add(mesh.bounds);
        }
    }

    public void OnDrag(PointerEventData data)
    {
        if ((CanDraw /*&& _pm.PlayerUnits.UnitsSelected.Count > 0 && DrawArrow) || (CanDraw && !DrawArrow*/))
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, Input.mousePosition,null, out Vector2 mouseLocalPoint);
            if (Vector2.Distance(lastLinePoint, mouseLocalPoint) > _minDistance)// Far enough from last point
            {

                //find forward vector
                Vector2 forwardVector = (mouseLocalPoint - lastLinePoint).normalized;
                //update last line point
                lastLinePoint = mouseLocalPoint;
                //update mesh
                MeshUtils.AddLinePoint(mesh, lastLinePoint, _lineWidth);
                lineCanvasRenderer.SetMesh(mesh);

                if (DrawArrow)
                {
	                //add point to positions
	                _positions.Add(new Vector3(lastLinePoint.x * _toWorldFactor + _mapZooming.CamInitial.x ,0,lastLinePoint.y * _toWorldFactor + _mapZooming.CamInitial.z));

	                //rotate arrow
	                arrowRectTransform.anchoredPosition = mouseLocalPoint;
	                arrowRectTransform.eulerAngles =
		                new Vector3(90, 0, UtilsClass.GetAngleFromVectorFloat(forwardVector));
                }
                else
	                bounds[bounds.Count-1] = mesh.bounds;
            }
        }
		else if(!CanDraw && _line != null && DrawArrow){//if player move cursor out
			// _pm.SetStackOfPositions(new List<Vector3>(_positions), _line);
		    _positions.Clear();
		    _line = null;
		}
    }

    public void OnEndDrag(PointerEventData eventData)
    {
	    if (CanDraw && _line != null && DrawArrow)
	    {
		    // _pm.SetStackOfPositions(new List<Vector3>(_positions), _line);
		    _positions.Clear();
		    _line = null;
	    }
    }

    public void Update()
    {
	    if (!DeleteNote || !Input.GetMouseButtonDown(0)) return;
	    RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, Input.mousePosition,null, out Vector2 mouseLocalPoint);
	    for (int i = 0; i < bounds.Count; i++)
	    {
		    if(bounds[i].Contains(mouseLocalPoint))
		    {
			    Destroy(_createdNotes[i]);
			    _createdNotes.RemoveAt(i);
			    bounds.RemoveAt(i);
			    return;
		    }
	    }
    }



}