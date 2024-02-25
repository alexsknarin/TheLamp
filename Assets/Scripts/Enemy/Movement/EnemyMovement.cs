using System;
using Unity.VisualScripting;
using UnityEngine;

public abstract class EnemyMovement : MonoBehaviour, IStateMachineOwner, IInitializable
{
    public event Action OnPreAttackStart;
    public event Action OnPreAttackEnd;

    public abstract void TriggerFall();
    public abstract void TriggerDeath();
    public abstract void TriggerAttack();
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
}
