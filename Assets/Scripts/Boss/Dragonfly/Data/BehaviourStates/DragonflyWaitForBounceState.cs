using System;

public class DragonflyWaitForBounceState : IState
{
    public event Action OnEnded;
    public void OnEnter()
    {
    }

    public void Tick()
    {
    }

    public void OnExit()
    {
        OnEnded?.Invoke();
    }
}
