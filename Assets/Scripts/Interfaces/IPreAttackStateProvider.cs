using System;

public interface IPreAttackStateProvider
{
    public event Action OnPreAttackStart;
    public event Action OnPreAttackEnd;
}
