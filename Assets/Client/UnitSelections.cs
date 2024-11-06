using UnityEngine;
using System.Collections.Generic;
public class UnitSelections : MonoBehaviour
{
    public List<GameObject> unitList = new List<GameObject>();
    public List<GameObject> unitSelected = new List<GameObject>();
    
    
    
    
    private static UnitSelections _instance;
    public static UnitSelections Instance { get { return _instance; } }

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

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
