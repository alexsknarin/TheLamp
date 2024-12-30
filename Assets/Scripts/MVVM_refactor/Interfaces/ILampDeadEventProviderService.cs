using System;

public interface ILampDeadEventProviderService
{
    public event Action<EnemyBase> OnLampDeadEvent;
}
