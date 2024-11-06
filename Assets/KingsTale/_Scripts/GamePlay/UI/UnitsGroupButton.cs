using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UnitsGroupButton : MonoBehaviour
{
    public List<UnitBrain> Group = new List<UnitBrain>();
    [SerializeField] private PlayerManager _player;
    [SerializeField] private TMP_Text _name, _name2, _unitsCount, _unitsCount2,_infantryCount,_artilleryCount,_magesCount,_cavalryCount,_supportCount;
    [SerializeField] private GameObject _info;

    private PlayerInput _input;

    private void Awake()
    {
        _input = _player.GetComponent<PlayerInput>();
    }
    public void SelectGroup()
    {
        // if (Group.Count == 0)
        // {
        //     for (int i = 0; i < _player.PlayerUnits.UnitsSelected.Count; i++)
        //     {
        //         Group.Add(_player.PlayerUnits.UnitsSelected[i]);
        //     }
        //
        //     UpdateText();
        //     _player.PlayerUnits.DeselctAll(_player.DrawArmy);
        // }
        // else if (Group.Count > 0)
        // {
        //     _player.PlayerUnits.DeselctAll(_player.DrawArmy);
        //     _player.PlayerUnits.UnitsSelected.Clear();
        //
        //     for (int i = 0; i < Group.Count; i++)
        //     {
        //         _player.PlayerUnits.DragSelect(Group[i]);
        //     }
        // }
    }
	public void UpdateText()
    {
        int infantry = 0, artillery = 0, mages = 0, cavalry = 0, support = 0;
        _unitsCount.text = "" + Group.Count;
        _unitsCount2.text = "" + Group.Count;
        foreach (var unit in Group)
        {
            if (unit.gameObject.name.StartsWith("Archer") || unit.gameObject.name.StartsWith("Knight") ||
                unit.gameObject.name.StartsWith("Legionnaire") || unit.gameObject.name.StartsWith("Paladin"))
                infantry += 1;
            else if (unit.gameObject.name.StartsWith("Pyromancer") || unit.gameObject.name.StartsWith("Icewizard") ||
                     unit.gameObject.name.StartsWith("Electrowizard"))
                mages += 1;
            else if (unit.gameObject.name.StartsWith("Ballist") || unit.gameObject.name.StartsWith("Catapult"))
                artillery += 1;
            else if (unit.gameObject.name.StartsWith("Priest") || unit.gameObject.name.StartsWith("Alchemist"))
                support += 1;
            else if (unit.gameObject.name.StartsWith("Cavalry"))
                cavalry += 1;
        }

        _infantryCount.text = "" + infantry;
        _artilleryCount.text = "" + artillery;
        _magesCount.text = "" + mages;
        _cavalryCount.text = "" + cavalry;
        _supportCount.text = "" + support;
    }

    public void ShowInfo()
    {
        if(_info.activeSelf)
            _info.SetActive(false);
        else
        {
            // transform.parent.GetComponent<CardsUI>().HideInfo();
            // _info.SetActive(true);
        }
    }

    public void ChangeName(string name)
    {
        _name.text = name;
        _name2.text = name;
    }

    public void Erase()
    {
        Group = new List<UnitBrain>();
        UpdateText();
        // _player.PlayerUnits.DeselctAll(_player.DrawArmy);
        // _player.PlayerUnits.UnitsSelected.Clear();
    }

    public void StopIteracting()
    {
        _input.enabled = false;
    }
    
    public void StartIteracting()
    {
        _input.enabled = true;
    }
}