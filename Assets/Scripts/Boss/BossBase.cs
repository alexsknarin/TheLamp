using System;
using UnityEngine;

public abstract class BossBase : EnemyBase
{
    public bool IsGameOver { get; set; }
    public static event Action OnTriggerSpreadEvent;
    public static event Action OnDeathEvent;
    public abstract void Reset();
    public abstract void Play();
    
    protected virtual void OnTriggerSpreadInvoke()
    {
        OnTriggerSpreadEvent?.Invoke();
    }
    
    protected virtual void OnDeathInvoke()
    {
        OnDeathEvent?.Invoke();
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
