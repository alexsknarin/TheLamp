using UnityEngine;

public interface IPositionDirectionProvider
{
    public Vector2 Position2D { get; }
    public Vector3 DepthDirection { get; }
}
