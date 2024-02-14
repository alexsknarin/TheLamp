using UnityEngine;

public interface IMovementState
{
    public void EnterState(Vector3 currentPosition, int sideDirection);
    public void ExecuteState(Vector3 currentPosition);
    public void CheckForStateChange();
    public void ExitState();
}
