

public class HealAbility : Ability
{
    public HealAbility(AbilityConfig config, EffectSystem fxSys, KingUnit king) : base(config, fxSys, king) { }
    
    public override void Use(AttackUnit unit = null)
    {
        if (!_canUse || unit == null || unit.gameObject.tag is not "Unit") return;
        
        var newFx = (EffectParameters)_effect.Clone();
        newFx.Target = unit;
        _fxSys.ApplyEffect(_fxSys.InstantHeal,newFx);
    }
}
