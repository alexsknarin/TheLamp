using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Dragonfly.BehaviourStates
{
    public class DragonflyPassiveState : IState
    {
        public void Enter()
        {
            Debug.Log("Collided and entered passive state.");
        }

        public void Tick() { }

        public void Exit() { }
    }
}
