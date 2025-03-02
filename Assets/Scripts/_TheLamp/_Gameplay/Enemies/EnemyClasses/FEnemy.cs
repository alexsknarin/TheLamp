using UnityEngine;
using UnityEngine.Pool;

public abstract class FEnemy: MonoBehaviour, IInitializable, IDamageable, IPoolableFEnemy, IAbleToAttack
{
    // Common fields
    protected bool _isInAttackReadyMovementState = false;
    protected IObjectPool<FEnemy> _objectPool;
    public virtual bool IsReadyToAttack { get; }
    public  bool IsReadyForDamage { get; protected set; }
    public  bool IsReceivedLampAttackDamage { get; protected set; }
    public bool IsDead { get; protected set; }
    public abstract void Initialize();
    public abstract void Play();
    public abstract void ReceiveDamage(int damageAmount);
    public abstract void Attack();
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
    public void ReturnToPool()
    {
        _objectPool.Release(this);
    }
}
