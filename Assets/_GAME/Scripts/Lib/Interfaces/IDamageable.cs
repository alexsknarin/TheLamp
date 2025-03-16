using System;

public interface IDamageable
{
    public bool IsReadyForDamage { get; }
    public bool IsReceivedLampAttackDamage { get; }
    public bool IsDead { get; }
    public void ReceiveDamage(int damageAmount);
}
