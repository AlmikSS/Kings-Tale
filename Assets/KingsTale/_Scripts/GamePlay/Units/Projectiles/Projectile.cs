using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [SerializeField] protected EffectParameters _effect;
    [SerializeField] protected float _autoDestroy = 5f;
    public float AutoDestroy => _autoDestroy;
    [HideInInspector] public Transform TargetObject;
    [HideInInspector] public AttackUnit Brain;
    
    protected EffectSystem _fxSys;
    void Start()
    {
        _fxSys = FindObjectOfType<EffectSystem>();
    }
    public abstract void Launch();
    public abstract void OnTriggerEnter(Collider other);
}
