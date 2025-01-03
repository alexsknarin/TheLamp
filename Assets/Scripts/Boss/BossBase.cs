using System;
using UnityEngine;

public abstract class BossBase : EnemyBase
{
    protected bool _isGameover = false;
    public static event Action SpreadTriggering;
    public static event Action BossDied;
    
    public virtual void SetGameOver()
    {
        _isGameover = true;
    }
    
    public abstract void Reset();
    
    public abstract void Play();

    public override void HandleEnteringAttackZone()
    {
        ReadyToLampDamage = true;
    }

    public override void HandleExitingAttackExitZone()
    {
        ReadyToLampDamage = false;
    }

    protected virtual void OnTriggerSpreadInvoke()
    {
        SpreadTriggering?.Invoke();
    }

    protected virtual void OnDeathInvoke()
    {
        BossDied?.Invoke();
    }
}
