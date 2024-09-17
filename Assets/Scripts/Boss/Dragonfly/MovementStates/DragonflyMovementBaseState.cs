using UnityEngine;

public abstract class DragonflyMovementBaseState : MonoBehaviour, IMovementState
{
    protected Transform _visibleBodyTransform;
    protected DragonflyMovement _owner;
    
    abstract public DragonflyStates State { get; }
    
    public virtual void SetCommonStateDependencies(DragonflyMovement owner, Transform visibleBodyTransform)
    {
        _owner = owner;
        _visibleBodyTransform = visibleBodyTransform;
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
