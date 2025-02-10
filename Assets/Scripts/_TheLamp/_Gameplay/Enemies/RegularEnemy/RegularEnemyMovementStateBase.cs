using UnityEngine;

public abstract class RegularEnemyMovementStateBase: IState
{
    public Vector2 Position2D { get; protected set; }
    public Vector3 DepthDirection { get; protected set; }
    public bool IsReadyToSwitch { get; protected set; }
    public abstract void OnEnter();
    public abstract void Tick();
    public virtual void OnExit() { }
}
