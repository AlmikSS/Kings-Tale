using UnityEngine;

[CreateAssetMenu(fileName = "AbilityConfig", menuName = "Configs/Abilities")]
public class AbilityConfig : ScriptableObject
{
    [HideInInspector] public bool Buyed;
    
    [Header("Settings")]
    [SerializeField] private AbilityName _name;
    public float Cooldown;
    public float Radius;
    public int Damage;
    public EffectParameters Effect;
    
    [Header("Visual")]
    [SerializeField] private GameObject _VFX;
    [SerializeField] private Animation _anim;
    
    public AbilityName Name => _name;
    public Animation Anim => _anim;
    public GameObject VFX => _VFX;
}

