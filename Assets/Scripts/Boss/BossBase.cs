using System;

public abstract class BossBase : EnemyBase
{
    public bool IsGameOver { get; set; }
    public static event Action OnTriggerSpread;
    public static event Action OnDeath;
    public abstract void Reset();
    public abstract void Play();
    
    protected virtual void OnTriggerSpreadInvoke()
    {
        OnTriggerSpread?.Invoke();
    }
    
    protected virtual void OnDeathInvoke()
    {
        OnDeath?.Invoke();
    }
    
    public override void HandleEnteringAttackZone()
    {
        ReadyToLampDamage = true;
    }
    
    public override void HandleExitingAttackExitZone()
    {
        ReadyToLampDamage = false;
    }
}
