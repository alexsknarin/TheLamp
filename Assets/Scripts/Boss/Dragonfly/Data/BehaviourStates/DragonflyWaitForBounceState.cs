using System;

public class DragonflyWaitForBounceState : IState
{
    public event Action OnEndedEvent;
    public void OnEnter()
    {
    }

    public void Tick()
    {
    }

    public void OnExit()
    {
        OnEndedEvent?.Invoke();
    }
}
