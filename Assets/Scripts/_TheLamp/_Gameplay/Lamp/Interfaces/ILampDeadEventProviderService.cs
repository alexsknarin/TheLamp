using System;

public interface ILampDeadEventProviderService
{
    public event Action LampDestroyed;
}
