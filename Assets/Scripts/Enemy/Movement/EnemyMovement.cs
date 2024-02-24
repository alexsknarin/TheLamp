using System;
using UnityEngine;

public abstract class EnemyMovement : MonoBehaviour, IStateMachineOwner
{
    public event Action OnPreAttackStart;
    public event Action OnPreAttackEnd;

    public abstract void TriggerFall();
    public abstract void TriggerDeath();
    public abstract void SwitchState();
}
