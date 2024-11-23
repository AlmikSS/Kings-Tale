using UnityEngine;
using System.Collections.Generic;

public class UnitSelections : MonoBehaviour
{
    public List<UnitBrain> unitList = new();
    public List<UnitBrain> unitSelected = new();

    public void DragSelect(UnitBrain unit)
    {
        if (!unitSelected.Contains(unit))
        {
            unitSelected.Add(unit);
            unit.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void Deselect()
    {
        foreach (var unit in unitSelected)
        {
            unit.transform.GetChild(0).gameObject.SetActive(false);
        }
        
        unitSelected.Clear();
    }
}
