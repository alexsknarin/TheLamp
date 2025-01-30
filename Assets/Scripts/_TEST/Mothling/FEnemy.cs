using UnityEngine;

public abstract class FEnemy: MonoBehaviour, ICollidableWithLamp, IInitializable, IDamageable
{
    public virtual float Radius { get; }
    public virtual Vector2 Position { get; }
    public virtual bool IsReadyToAttack { get; }
    public  bool IsCollided { get; protected set; }
    public  CollidableState CollisionState { get; protected set; }
    public  bool IsReadyForDamage { get; protected set; }
    public  bool IsReceivedAttack { get; protected set; }
    
    public abstract void Initialize();
    public abstract void Play();
    public abstract void HandleEnterAttackZone();
    public abstract void HandleCollision();
    public abstract void HandleExitAttackZone();
    public abstract Vector3 ProvideImpactPoint();
    public abstract void ReceiveDamage(int damageAmount);
    public abstract void Attack();
    public abstract void Spread();
    public abstract void DoDeath();


}
