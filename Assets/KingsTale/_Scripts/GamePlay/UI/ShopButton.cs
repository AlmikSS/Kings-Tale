using UnityEngine;
using UnityEngine.Localization.Components;

public class ShopButton : MonoBehaviour
{
    [SerializeField] private BuildingBaseConfigSO _building;
    [SerializeField] private LocalizeStringEvent _name;
    [SerializeField] private LocalizeStringEvent _descripiton;
    private void Start()
    {
        _name.StringReference = _building.Name;
        _descripiton.StringReference = _building.Description;
    }
}
