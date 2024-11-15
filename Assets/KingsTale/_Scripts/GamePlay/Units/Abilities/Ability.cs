using System;
using System.Threading.Tasks;
using UnityEngine;

public abstract class Ability
{
    protected bool _buyed;
    
    protected float _cooldown;
    protected float _radius;
    protected int _damage;
    protected EffectParameters _effect;
    protected GameObject _VFX;
    
    protected bool _canUse = true;
    protected EffectSystem _fxSys;
    protected KingUnit _king;
    
    public Ability(AbilityConfig config, EffectSystem fxSys, KingUnit king)
    {
        if (config == null) return;
        
        _buyed = config.Buyed;
        _cooldown = config.Cooldown;
        _radius = config.Radius;
        _damage = config.Damage;
        _effect = config.Effect;
        _VFX = config.VFX;
        _fxSys = fxSys;
        _king = king;
    }
    
    public virtual void Use(AttackUnit unit = null){}
    public virtual void DeUse(AttackUnit unit = null){}
    
    public async void Cooldown()
    {
        _canUse = false;
        await Task.Delay((int)_cooldown*1000);
        _canUse = true;
    }
    
}

public enum AbilityName
{
    FireAttack,
    FireRage,
    FireShield,
    HolyHeal,
}
