using UnityEngine;
using System.Collections.Generic;

public class UnitSelections : MonoBehaviour
{
    public List<GameObject> unitList = new();
    public List<GameObject> unitSelected = new();

    public void SelectUnit(GameObject unit)
    {
        Deselect();
        if (unitList.Contains(unit))
        {
            unitSelected.Add(unit);
        }
    }

    public void DragSelect(GameObject unit)
    {
        if (!unitSelected.Contains(unit))
        {
            unitSelected.Add(unit);
        }
    }

    public void Deselect()
    {
        unitSelected.Clear();
    }
}
