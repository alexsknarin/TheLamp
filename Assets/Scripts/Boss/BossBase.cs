using System;
using UnityEngine;

public abstract class BossBase : EnemyBase
{
    protected bool _isGameover = false;
    public static event Action OnTriggerSpreadEvent;
    public static event Action OnDeathEvent;
    
    public virtual void SetGameover()
    {
        _isGameover = true;
    }
    
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
