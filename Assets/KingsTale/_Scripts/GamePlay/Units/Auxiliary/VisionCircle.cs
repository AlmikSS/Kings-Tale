using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VisionCircle : MonoBehaviour
{
    private AttackUnit _unitBrain => transform.parent.GetComponent<AttackUnit>();
    private MoveGroupManager _groupManager;// => _unitBrain.PlayerMng.GroupManager;
    public Building BuildingToAttack;
    private void OnTriggerEnter(Collider other)
    {
        if (!other.gameObject.transform.parent ||
            !other.gameObject.transform.parent.gameObject.TryGetComponent(out EnemyUnit unit) ||
            _unitBrain._objectToAttack) return;
        
        if(unit.transform.parent.TryGetComponent(out EnemyGroups group)){
            var brains = _groupManager.CheckAgent(_unitBrain);
            if (_unitBrain.AttackConfig.IsLongRange)
            {
                _unitBrain.Complete();
                if(_unitBrain._objectToAttack)
                    return;
            }
            else if (brains != null)
            {
                foreach (var _brain in brains.Select(brain => brain.GetComponent<AttackUnit>()))
                {
                    _brain.StackAttackObjects = group;
                    _brain.SearchForEnemy();
                }
            }
            _unitBrain.StackAttackObjects = group;
            _unitBrain.SearchForEnemy();	
                
        }
        else{
            _unitBrain.StackAttackObjects = null;
            _unitBrain.StartAttack(unit.gameObject);
                
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!_unitBrain.AttackConfig.IsLongRange || _unitBrain._objectToAttack || !BuildingToAttack ||
            BuildingToAttack.gameObject != other.gameObject) return;
        _unitBrain.Complete();
        _unitBrain.StartAttack(BuildingToAttack.gameObject);
        BuildingToAttack = null;
    }

    public IEnumerator Deactivate()
    {
        gameObject.SetActive(false);
        yield return new WaitForSeconds(5f);
        gameObject.SetActive(true);
    }
}
