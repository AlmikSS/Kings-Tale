using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class DrawArmy : NetworkBehaviour
{
    [Header("Player")]
    [SerializeField] private PlayerManager _player;
    [SerializeField] private Camera _cam;

    [Header("LayerMasks")]
    [SerializeField] private LayerMask _planeMask;
    [SerializeField] private LayerMask _buildingsMask;

    [Header("Settings")]
    [SerializeField] private float _distanceBtwUnits = 1f;

    public GameObject _fantomUnit;

    [HideInInspector] public int i;
    private Vector2 _lastMousePos;
    private Dictionary<GameObject, GameObject> _fantoms = new();
    
    private void Update()
    {
        //if (!IsOwner) { return; }
        if (_fantomUnit && Input.GetMouseButtonDown(0))
            _lastMousePos = Input.mousePosition;


        if (!_fantomUnit || !Input.GetMouseButton(0) ||
            !(Vector2.Distance(_lastMousePos, Input.mousePosition) > _distanceBtwUnits)) return;
        
        _lastMousePos = Input.mousePosition;
        Ray ray = _cam.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out var hit, Mathf.Infinity, _planeMask) ||
            Physics.Raycast(ray, out _, Mathf.Infinity, _buildingsMask)) return;
        
        // if (_player.PlayerUnits.UnitsSelected.Count > i)
        // {
        //     var curretKey = _player.PlayerUnits.UnitsSelected[i].gameObject;
        //     if (_fantoms.TryGetValue(curretKey, out var fantom))
        //     {
        //         Destroy(fantom);
        //         _fantoms.Remove(curretKey);
        //     }
        //     
        //     GameObject obj = Instantiate(_fantomUnit, hit.point, Quaternion.identity);
        //     _fantoms.Add(curretKey, obj);
        //     
        //     //obj.GetComponent<NetworkObject>().Spawn(true);
        //     _player.PlayerUnits.UnitsSelected[i].GoToPoint(obj.transform.position);
        //     if(_player.PlayerUnits.UnitsSelected[i].GetComponent<AttackUnit>()){
        //         var _attackUnit = _player.PlayerUnits.UnitsSelected[i].GetComponent<AttackUnit>();
        //         _attackUnit.StackAttackObjects = null;
        //         _attackUnit.StopAttack();
        //         if(!_attackUnit.AttackConfig.IsLongRange)
        //             StartCoroutine(_attackUnit.transform.Find("EnemyVision").GetComponent<VisionCircle>().Deactivate());
        //     }
							 //
        //     StartCoroutine(IsComeToPoint(_player.PlayerUnits.UnitsSelected[i].transform, obj.transform));
        //     Destroy(obj, 15f);
        //     i++;
        // }
        // else
        // {
        //     _player.PlayerUnits.DeselctAll(this);
        //     _player.CardsUIObj.DeselectAll();
        // }
    }

    private IEnumerator IsComeToPoint(Transform unit, Transform point)
    {
        if (point && Vector3.Distance(unit.position, point.position) > 1f)
        {
            yield return new WaitForSeconds(0.2f);
            StartCoroutine(IsComeToPoint(unit, point));
        }
        else
        {
            //point.gameObject.GetComponent<NetworkObject>().Despawn(true);
            Destroy(point.gameObject);
        }
    }
}