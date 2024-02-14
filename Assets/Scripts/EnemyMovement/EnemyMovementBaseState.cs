using UnityEngine;
using UnityEngine.UIElements;

public abstract class EnemyMovementBaseState : IMovementState
{
    public Vector3 Position { get; protected set; }
    abstract public EnemyStates State { get; protected set;}

    protected int _sideDirection;
    protected float _speed;
    protected float _radius;
    protected float _verticalAmplitude;

    protected EnemyBaseMovement _owner;

    public EnemyMovementBaseState()
    {
    }

    public virtual void EnterState(Vector3 currentPosition, int sideDirection)
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
