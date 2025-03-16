using System;

namespace _GAME.Scripts.Lib.Interfaces
{
    public interface ILampDeadEventProviderService
    {
        public event Action LampDestroyed;
    }
}
