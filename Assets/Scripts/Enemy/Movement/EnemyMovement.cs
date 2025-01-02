using System;
using UnityEngine;

public abstract class EnemyMovement : MonoBehaviour, IStateMachineOwner, IInitializable
{
    public EnemyState State { get; protected set; }
    public int SideDirection { get; protected set; }
    public event Action PreAttackStarted;
    public event Action PreAttackEnded;
    public event Action AttackEnded;
    public event Action StickStarted;
    public event Action EnemyDeactivated; 
    public event Action MovementReseted;

    public virtual void Construct(ILampPositionProviderService lampPositionProviderService) { }
    public abstract void Initialize();
    public virtual void HandleLampDestroyed() { }
    public abstract void TriggerFall();
    public abstract void TriggerDeath();
    public abstract void TriggerAttack();
    public abstract void TriggerSpread();
    public abstract void TriggerStick();
    public abstract void SwitchState();

    protected virtual void OnPreAttackStartInvoke()
    {
        PreAttackStarted?.Invoke();
    }

    protected virtual void OnPreAttackEndInvoke()
    {
        PreAttackEnded?.Invoke();
    }
    
    protected virtual void OnAttackEndInvoke()
    {
        AttackEnded?.Invoke();
    }
    
    protected virtual void OnStickStartInvoke()
    {
        StickStarted?.Invoke();
    }
   
    protected virtual void OnEnemyDeactivatedInvoke()
    {
        EnemyDeactivated?.Invoke();
    }
    
    protected virtual void OnMovementResetInvoke()
    {
        MovementReseted?.Invoke();
    }
}
