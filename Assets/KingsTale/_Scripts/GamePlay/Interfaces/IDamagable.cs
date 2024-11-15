public interface  IDamagable
{
    public void TakeDamage(int damage, DamageType type = 0, Effect fx = Effect.None);
    public void Die();
}
