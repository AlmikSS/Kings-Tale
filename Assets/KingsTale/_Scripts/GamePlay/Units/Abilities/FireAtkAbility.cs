public class FireAtkAbility : Ability
{
    public FireAtkAbility(AbilityConfig config, EffectSystem fxSys, KingUnit king) : base(config, fxSys, king) { }
    
    public override void Use(AttackUnit unit = null)
    {
        if (!_canUse || unit == null || unit.gameObject.tag is not "EnemyUnit") return;

        unit.TakeDamage(_damage);
        var newFx = (EffectParameters)_effect.Clone();
        newFx.Target = unit;
        _fxSys.ApplyEffect(_fxSys.Burn, newFx);
    }
}
