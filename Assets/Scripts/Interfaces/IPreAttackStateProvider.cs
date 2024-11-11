using System;

public interface IPreAttackStateProvider
{
    public event Action OnPreAttackStartEvent;
    public event Action OnPreAttackEndEvent;
}
