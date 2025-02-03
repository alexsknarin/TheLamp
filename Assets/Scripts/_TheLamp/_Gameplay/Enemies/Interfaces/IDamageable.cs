using System;

public interface IDamageable
{
    public bool IsReadyForDamage { get; }
    public bool IsReceivedAttack { get; }
    public void ReceiveDamage(int damageAmount);
}
