using UnityEngine;

public abstract class DragonflyMovementBaseState : MonoBehaviour, IMovementState
{
    abstract public DragonflyStates State { get; }
    
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
