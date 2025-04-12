using System;
using _GAME.Scripts.Lib.Interfaces;

namespace _GAME.Scripts.Enemies.Dragonfly.BehaviourStates
{
    public class DragonflyWaitForBounceState : IState
    {
        public event Action Ended;
        public void Enter()
        {
        }

        public void Tick()
        {
        }

        public void Exit()
        {
            Ended?.Invoke();
        }
    }
}
