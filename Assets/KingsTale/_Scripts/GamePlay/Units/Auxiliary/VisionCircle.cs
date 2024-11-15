using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VisionCircle : MonoBehaviour
{
    private AttackUnit _unitBrain => transform.parent.GetComponent<AttackUnit>();
    public Building BuildingToAttack;
    private void OnTriggerEnter(Collider other)
    {
        ////////////////ADD SERVER UNIT CHECK
        if (!other.gameObject.transform.parent ||
            !other.gameObject.transform.parent.gameObject.TryGetComponent(out UnitBrain unit) ||
            _unitBrain._objectToAttack) return;
        _unitBrain.StartAttack(unit.gameObject);
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
