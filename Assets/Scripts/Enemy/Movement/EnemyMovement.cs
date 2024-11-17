using System;
using UnityEngine;

public abstract class EnemyMovement : MonoBehaviour, IStateMachineOwner, IInitializable
{
    public EnemyState State { get; protected set; }
    public int SideDirection { get; protected set; }
    public event Action OnPreAttackStartEvent;
    public event Action OnPreAttackEndEvent;
    public event Action OnAttackEndEvent;
    public event Action OnStickStartEvent;
    public event Action OnEnemyDeactivatedEvent; 
    public event Action OnMovementResetEvent;

    public abstract void Initialize();
    public abstract void TriggerFall();
    public abstract void TriggerDeath();
    public abstract void TriggerAttack();
    public abstract void TriggerSpread();
    public abstract void TriggerStick();
    public abstract void SwitchState();

    protected virtual void OnPreAttackStartInvoke()
    {
        OnPreAttackStartEvent?.Invoke();
    }

    protected virtual void OnPreAttackEndInvoke()
    {
        OnPreAttackEndEvent?.Invoke();
    }
    
    protected virtual void OnAttackEndInvoke()
    {
        OnAttackEndEvent?.Invoke();
    }
    
    protected virtual void OnStickStartInvoke()
    {
        OnStickStartEvent?.Invoke();
    }
   
    protected virtual void OnEnemyDeactivatedInvoke()
    {
        OnEnemyDeactivatedEvent?.Invoke();
    }
    
    protected virtual void OnMovementResetInvoke()
    {
        OnMovementResetEvent?.Invoke();
    }
}
