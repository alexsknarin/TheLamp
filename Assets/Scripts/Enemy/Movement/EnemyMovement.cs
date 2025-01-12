using System;
using UnityEngine;

public abstract class EnemyMovement : MonoBehaviour, IStateMachineOwner, IInitializable
{
    public virtual void Construct(ILampPositionProviderService lampPositionProviderService) { }
    public event Action PreAttackStarted;
    public event Action PreAttackEnded;
    public event Action AttackEnded;
    public event Action StickStarted;
    public event Action EnemyDeactivated; // TODO: rename to Enemy Death State Ended or something like that - movement shouldn't know anything about active-inactive states
    public event Action MovementReseted;
    public EnemyState State { get; protected set; }
    public int SideDirection { get; protected set; }

    public abstract void Initialize();
    public virtual void HandleLampDestroyed() { }
    public abstract void TriggerFall();
    public abstract void TriggerDeath();
    public abstract void TriggerAttack();
    public abstract void TriggerSpread();
    public abstract void TriggerStick(); // TODO: move it into a separate interface or reimplement in the ladybug movement specifically
    public abstract void SwitchState();

    protected void OnPreAttackStartInvoke()
    {
        PreAttackStarted?.Invoke();
    }

    protected void OnPreAttackEndInvoke()
    {
        PreAttackEnded?.Invoke();
    }
    
    protected void OnAttackEndInvoke()
    {
        AttackEnded?.Invoke();
    }
    
    protected void OnStickStartInvoke()
    {
        StickStarted?.Invoke();
    }
   
    protected void OnEnemyDeactivatedInvoke()
    {
        EnemyDeactivated?.Invoke();
    }
    
    protected void OnMovementResetInvoke()
    {
        MovementReseted?.Invoke();
    }
}
