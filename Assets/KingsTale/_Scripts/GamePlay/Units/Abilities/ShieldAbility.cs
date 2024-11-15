

public class ShieldAbility : Ability
{
    public ShieldAbility(AbilityConfig config, EffectSystem fxSys, KingUnit king) : base(config, fxSys, king) { }
    
    public override void Use(AttackUnit unit = null)
    {
        if (!_canUse || unit == null || unit.gameObject.tag is not "Unit") return;
        
        var newFx = (EffectParameters)_effect.Clone();
        newFx.Target = unit;
        _fxSys.ApplyEffect(_fxSys.FireShield, newFx);
    }
    public override void DeUse(AttackUnit unit = null)
    {
        if (!_canUse || unit == null || unit.gameObject.tag is not "Unit") return;
        
        var newFx = (EffectParameters)_effect.Clone();
        newFx.Target = unit;
        _fxSys.ApplyEffect(_fxSys.FireShield, newFx);
    }
}
