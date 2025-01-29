using UnityEngine;

public interface ICollidableWithLamp
{
    public float Radius { get; }
    public Vector2 Position { get; }
    public bool IsCollided { get; } // ????
    public CollidableState CollisionState { get; }
    public void HandleEnterAttackZone();
    public void HandleCollision();
    public void HandleExitAttackZone();
}
