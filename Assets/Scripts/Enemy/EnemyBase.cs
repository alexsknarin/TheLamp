using System;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IInitializable
{
    public static event Action<EnemyBase> EnemyDied;
    public virtual EnemyType EnemyType { get; protected set; }
    public bool IsAttacking { get; protected set; }
    public bool IsStick { get; protected set; }
    public bool ReadyToAttack { get; protected set; }
    public bool ReadyToCollide { get; protected set; }
    public bool ReadyToLampDamage { get; protected set; }
    public bool ReceivedLampAttack { get; protected set; }
    public virtual void Initialize() {}
    public virtual void HandleLampDestroyed() {}
    public abstract void SpreadStart();
    public abstract void StartAttack();

    public virtual void HandleEnteringAttackZone()
    {
        ReadyToLampDamage = true;
    }

    public abstract void HandleCollisionWithLamp();
    public virtual void HandleExitingAttackExitZone()
    {
        ReadyToLampDamage = false;
    }
    public abstract void HandleCollisionWithStickZone();
    public abstract void ReceiveDamage(int damage);
    public abstract void UpdateAttackAvailability();
    public abstract void ReturnToPool();
    public abstract Vector3 ProvideImpactPoint();
    
    protected virtual void OnEnemyDeathInvoke(EnemyBase enemy)
    {
        EnemyDied?.Invoke(enemy);
    }
}
