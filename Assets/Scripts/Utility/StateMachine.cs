public class StateMachine
{
    private IState _currentState;

    public void SetState(IState state)
    {
        if (_currentState != null)
        {
            _currentState.ExitState();
        }
        _currentState = state;
        _currentState.EnterState();
    }

    public void Execute()
    {
        _currentState.ExecuteState();
    }
}
