using System.Collections;
using UnityEngine;

public class KnightUnit : AttackUnit
{
    protected override IEnumerator AttackTargetRoutine()
    {
        
        yield return new WaitForSeconds(_attackSpeed);
    }
}