using UnityEngine;

public abstract class DragonflyMovementBaseState : ScriptableObject, IMovementState
{
    protected DragonflyMovementStateData _stateData;
    
    abstract public DragonflyState State { get; }
    
    public virtual void SetCommonStateDependencies(DragonflyMovementStateData stateData)
    {
        _stateData = stateData;
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
