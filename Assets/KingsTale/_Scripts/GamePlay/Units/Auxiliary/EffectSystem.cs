using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class EffectSystem : MonoBehaviour
{
    public delegate IEnumerator EffectIE(EffectParameters param);
    
    private IEnumerator TakeDmg(EffectParameters param)
    {
        if((param.Target && !param.Target.Effects.Contains(param.Name)) || (param.TargetBulding && !param.TargetBulding.Effects.Contains(param.Name))) yield break;
 
        float time = 0f;
        
        while(time < param.DurationTime){
            
            time += param.TickTime;
            
            if (param.Target)
                param.Target.TakeDamage(param.Damage, DamageType.Magical, param.Name);
            else
                param.TargetBulding.TakeDamage(param.Damage, DamageType.Magical, param.Name);
            
            yield return new WaitForSeconds(param.TickTime);
        }
        if(param.Target)
            param.Target.Effects.Remove(param.Name);
        else
            param.TargetBulding.Effects.Remove(param.Name);
    }
    
    public IEnumerator Burn(EffectParameters param)
    {
        if((param.Target && param.Target.Effects.Contains(param.Name)) || (param.TargetBulding && param.TargetBulding.Effects.Contains(param.Name))) yield break;
        if (param.Target)
            param.Target.Effects.Add(param.Name);
        else
            param.TargetBulding.Effects.Add(param.Name);
        StartCoroutine(TakeDmg(param));
        
        yield return null;
    }
    
    
    public IEnumerator Freeze(EffectParameters param)
    {
        if(!param.Target || (param.Target && param.Target.Effects.Contains(param.Name))) yield break;
        
        param.Target.Effects.Add(param.Name);
        var speed = param.Target.Config.Speed;
        param.Target.GetComponent<NavMeshAgent>().speed = 0;
        if(param.Damage > 0)
            param.Target.TakeDamage(param.Damage, DamageType.Magical, param.Name);
        
        yield return new WaitForSeconds(param.DurationTime);
        if(param.Target)
            param.Target.GetComponent<NavMeshAgent>().speed = speed;
        yield return new WaitForSeconds(2f);
        if(param.Target)
            param.Target.Effects.Remove(param.Name);
    }
    
    public IEnumerator DecreaseMagic(EffectParameters param)
    {
        if(param.Target && !param.Target.Effects.Contains(param.Name))
            StartCoroutine(param.Target.ChangeParam(Effect.Alchemist, param.AdditEffect, param.DurationTime, false));
        else if(param.TargetBulding && !param.TargetBulding.Effects.Contains(param.Name))
            StartCoroutine(param.TargetBulding.ChangeParam(Effect.Alchemist, param.AdditEffect, param.DurationTime, false));
        yield return null;
    }
    
    public IEnumerator MicroStan(EffectParameters param)
    {
        if(!param.Target || (param.Target && param.Target.Effects.Contains(param.Name))) yield break;

        var speed = param.Target.Config.Speed;
        param.Target.GetComponent<NavMeshAgent>().speed = 0;
        yield return new WaitForSeconds(1f);
        param.Target.GetComponent<NavMeshAgent>().speed = speed;
        
    }
    public IEnumerator InstantHeal(EffectParameters param)
    {
        StartCoroutine(param.Target.ChangeParam(Effect.InstantHeal, param.AdditEffect, 0));
        yield return null;
    }
    public IEnumerator FireRage(EffectParameters param)
    {
        var list = param.Target.Effects;
        if (list.Contains(param.Name))
            list.Remove(param.Name);
        else
            list.Add(param.Name);
        yield return null;
    }
    public IEnumerator FireShield(EffectParameters param)
    {
        StartCoroutine(param.Target.ChangeParam(Effect.FireShield, param.AdditEffect, 0));
        yield return null;
    }
    

    public void ApplyEffect(EffectIE fx, EffectParameters param) => StartCoroutine(fx(param));

    public void DisableEffect(Effect fx, UnitBrain target = null, Building targetBulding = null)
    {
        if(target)
            target.Effects.Remove(fx);
        else
            targetBulding.Effects.Remove(fx);
    }
    public void IncreaseRegen(int factor, UnitBrain target, bool increase = true) => StartCoroutine(target.ChangeParam(Effect.Heal, factor, 0, increase));

    
}

[Serializable]
public class EffectParameters : ICloneable
{
    public Effect Name => _name;

    public int Damage => _damage;

    public float DurationTime => _durationTime;

    public float TickTime => _tickTime;

    public int AdditEffect => _additEffect;

    [SerializeField] private Effect _name;
    [SerializeField] private int _damage;
    [SerializeField] private float _durationTime;
    [SerializeField] private float _tickTime;
    [SerializeField] private int _additEffect;
    [HideInInspector] public UnitBrain Target;
    [HideInInspector] public Building TargetBulding;

    public EffectParameters(Effect name, int damage, float durationTime, float tickTime, int additEffect, UnitBrain target = null, Building targetBulding = null)
    {
        _name = name;
        _damage = damage;
        _durationTime = durationTime;
        _tickTime = tickTime;
        _additEffect = additEffect;
        Target = target;
        TargetBulding = targetBulding;
    }
    public object Clone()
    {
        return new EffectParameters(_name,_damage,_durationTime,_tickTime,_additEffect, Target, TargetBulding);
    }
}

public enum Effect
{
    None,
    Pyro,
    Electro,
    Freeze,
    Heal,
    Alchemist,
    InstantHeal,
    FireRage,
    FireShield,
}

public enum DamageType
{
    Physical,
    Magical
}
