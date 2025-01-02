using System;

public class DragonflyWaitForBounceState : IState
{
    public event Action Ended;
    public void OnEnter()
    {
    }

    public void Tick()
    {
    }

    public void OnExit()
    {
        Ended?.Invoke();
    }
}
