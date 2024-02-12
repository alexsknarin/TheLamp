using UnityEngine;

public class EnemyMovementStateMachine
{
    private IMovementState _currentState;

    public void SetState(IMovementState state, Vector3 currentPosition)
    {
        if (_currentState != null)
        {
            _currentState.ExitState();
        }
        _currentState = state;
        _currentState.EnterState(currentPosition);
    }

    public void Execute(Vector3 currentPosition)
    {
        _currentState.ExecuteState(currentPosition);
    }
    
    public void CheckForStateChange()
    {
        _currentState.CheckForStateChange();
    }
}
