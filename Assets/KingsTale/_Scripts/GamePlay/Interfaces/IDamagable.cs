using Unity.Netcode;

public interface  IDamagable
{
    public int Health { get; }

    [Rpc(SendTo.Owner)]
    public void TakeDamageRpc(int damage);
    public void Die();
}
