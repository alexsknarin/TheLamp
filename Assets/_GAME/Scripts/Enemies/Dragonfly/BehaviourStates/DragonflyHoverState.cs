using System;
using _GAME.Scripts.Lib.Interfaces;

namespace _GAME.Scripts.Enemies.Dragonfly.BehaviourStates
{
    public class DragonflyHoverState : IState
    {
        public event Action Started;

        public void Enter()
        {
            Started?.Invoke();
        }

        public void Tick() { }

        public void Exit() { }
    }
}
