using UnityEngine;
using UnityEngine.Pool;

public abstract class FEnemy: MonoBehaviour, ICollidableWithLamp, IInitializable, IDamageable, IPoolableFEnemy, IAbleToAttack
{
    // Common fields
    protected bool _isInAttackReadyMovementState = false;
    protected IObjectPool<FEnemy> _objectPool;
    
    public virtual float Radius { get; }
    public virtual Vector2 Position { get; }
    public virtual bool IsReadyToAttack { get; }
    public  bool IsCollided { get; protected set; }
    public  CollidableState CollisionState { get; protected set; }
    public  bool IsReadyForDamage { get; protected set; }
    public  bool IsReceivedAttack { get; protected set; }
    
    public abstract void Initialize();
    public abstract void Play();

    public virtual void HandleEnterAttackZone()
    {
        IsCollided = false;
        CollisionState = CollidableState.InAttackZone;
        IsReadyForDamage = true;
    }
    
    public abstract void HandleCollision();

    public virtual void HandleExitAttackZone()
    {
        CollisionState = CollidableState.Outside;
        IsCollided = false;
        IsReadyForDamage = false;
    }

    public virtual Vector3 ProvideImpactPoint()
    {
        return transform.position;
    }

    public abstract void ReceiveDamage(int damageAmount);
    public abstract void Attack();
    public abstract void Spread();
    public abstract void DoDeath();

    public virtual void SetObjectPool(ObjectPool<FEnemy> pool)
    {
        _objectPool = pool;
    }
    
    // Common event handlers 
    protected void OnReadyToAttackStateStarted()
    {
        _isInAttackReadyMovementState = true;
    }

    protected void OnReadyToAttackStateEnded()
    {
        _isInAttackReadyMovementState = false;
    }
    
    protected void OnDeathStateEnded()
    {
        _objectPool.Release(this);
    }
}
