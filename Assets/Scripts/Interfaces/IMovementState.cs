using UnityEngine;

public interface IMovementState
{
    public void EnterState();
    public void ExecuteState(Vector3 currentPosition);
    public void CheckForStateChange();
    public void ExitState();
}
