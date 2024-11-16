using UnityEngine;

public class FieldBuilding : Building
{
    [SerializeField] private GameObject _wheat;
    [SerializeField] private float _resetTime;

    public bool IsGrew { get; private set; } = true;
    public bool HasAsigned { get; private set; }

    public void Asign() => HasAsigned = true;
    
    public void Collect()
    {
        HasAsigned = false;
        IsGrew = false;
        _wheat.SetActive(false);
        Invoke(nameof(ResetWheat), _resetTime);
    }

    private void ResetWheat()
    {
        _wheat.SetActive(true);
        IsGrew = true;
    }
}