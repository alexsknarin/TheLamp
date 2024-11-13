public class StateMachine
{
    private IState _currentState;

    public void SetState(IState state)
    {
        if (_currentState != null)
        {
            _currentState.OnExit();
        }
        _currentState = state;
        _currentState.OnEnter();
    }

    public void Execute()
    {
        _currentState.Tick();
    }
}
