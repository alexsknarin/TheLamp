using System;
using UnityEngine;

public abstract class EnemyMovement : MonoBehaviour, IStateMachineOwner, IInitializable
{
    public EnemyStates State { get; protected set; }
    public event Action OnPreAttackStart;
    public event Action OnPreAttackEnd;
    public event Action OnStateChanged; 
    public event Action OnEnemyDeactivated; 

    public abstract void TriggerFall();
    public abstract void TriggerDeath();
    public abstract void TriggerAttack();
    public abstract void TriggerStick();
    public abstract void SwitchState();
    public abstract void Initialize();

    protected virtual void OnPreAttackStartInvoke()
    {
        OnPreAttackStart?.Invoke();
    }

    protected virtual void OnPreAttackEndInvoke()
    {
        OnPreAttackEnd?.Invoke();
    }
    
    protected virtual void OnStateChangeInvoke()
    {
        OnStateChanged?.Invoke();
    }
    
    protected virtual void OnEnemyDeactivatedInvoke()
    {
        OnEnemyDeactivated?.Invoke();
    }
}
