using UnityEngine;

public abstract class FMothlingMovementStateBase: IState
{
    // Output parameters
    public Vector2 Position2D { get; protected set; }
    public Vector3 DepthDirection { get; protected set; }
    public bool ReadyToSwitch { get; protected set; }
    public abstract void OnEnter();
    public abstract void Tick();
    public virtual void OnExit() { }
}
