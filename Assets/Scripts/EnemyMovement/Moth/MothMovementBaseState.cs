using UnityEngine;
using UnityEngine.UIElements;

public abstract class MothMovementBaseState : IMovementState
{
    public Vector3 Position { get; protected set; }
    public Vector3 Depth { get; protected set; }
    abstract public EnemyStates State { get; }
    
    protected readonly Vector3 _cameraPosition = new Vector3(0, 0, -5.88f);

    protected int _sideDirection;
    protected int _depthDirection;
    protected float _speed;
    protected float _radius;
    protected float _verticalAmplitude;

    protected MothMovement _owner;

    public MothMovementBaseState()
    {
    }

    public virtual void EnterState(Vector3 currentPosition, int sideDirection, int depthDirection)
    {
    }

    public virtual void ExecuteState(Vector3 currentPosition)
    {
    }

    public virtual void CheckForStateChange()
    {
    }

    public virtual void ExitState()
    {
    }
}