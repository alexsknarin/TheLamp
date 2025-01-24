using UnityEngine;

public interface ICollidable
{
    public float Radius { get; }
    public Vector2 Position { get; }
    public void HandleCollision(Vector2 newPosition);
}
