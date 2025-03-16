using System;
using _GAME.Scripts.Lib.Interfaces;

namespace _GAME.Scripts.Enemies.Dragonfly.BehaviourStates
{
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
}
