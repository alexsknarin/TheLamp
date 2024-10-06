using System;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IInitializable
{
    public static event Action<EnemyBase> OnEnemyDeath;
    public virtual EnemyTypes EnemyType { get; protected set; }
    public bool IsAttacking { get; protected set; }
    public bool IsStick { get; protected set; }
    public bool ReadyToAttack { get; protected set; }
    public bool ReadyToCollide { get; protected set; }
    public bool ReadyToLampDamage { get; protected set; }
    public bool ReceivedLampAttack { get; protected set; }

    
    public virtual void Initialize()
    {
    }
    public abstract void SpreadStart();
    public abstract void AttackStart();
    public abstract void HandleEnteringAttackZone(Collider2D collider);
    public abstract void HandleCollisionWithLamp();
    public abstract void HandleExitingAttackExitZone();
    public abstract void HandleCollisionWithStickZone();
    public abstract void ReceiveDamage(int damage);
    public abstract void UpdateAttackAvailability();
    public abstract void ReturnToPool();
    
    protected virtual void OnEnemyDeathInvoke(EnemyBase enemy)
    {
        OnEnemyDeath?.Invoke(enemy);
    }
}
