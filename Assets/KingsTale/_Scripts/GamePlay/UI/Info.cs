using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
public class Info : MonoBehaviour
{
    [SerializeField] private LocalizeStringEvent _nameOfBuilding;
    [SerializeField] private LocalizeStringEvent _infoOfBuilding;

    public void ShowInfo(LocalizedString name, LocalizedString info)
    {
        _nameOfBuilding.StringReference = name;
        _infoOfBuilding.StringReference = info;
    }
}