using UnityEngine;

public abstract class CollidableEnemy: FEnemy, ICollidableWithLamp
{
    public virtual  Vector2 Position { get; }
    public virtual float Radius { get; }
    public bool IsCollided { get; protected set; }
    public CollidableState CollisionState { get; protected set; }
    
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
}
