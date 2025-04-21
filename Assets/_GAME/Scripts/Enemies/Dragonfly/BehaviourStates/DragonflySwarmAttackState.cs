using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Dragonfly.BehaviourStates
{
    public class DragonflySwarmAttackState : IState
    {
        private readonly float _duration;
        private float _localTime;
        public bool ReadyToSwitch;
    
        public DragonflySwarmAttackState(float duration)
        {
            _duration = duration;
        }
        
        public event Action Started;
        
        public void Enter()
        {
             ReadyToSwitch = false;
            _localTime = 0f;
            Started?.Invoke();
        }

        public void Tick()
        {
            _localTime += Time.deltaTime;
            if (_localTime >= _duration)
            {
                ReadyToSwitch = true;
            }
        }

        public void Exit()
        {
            ReadyToSwitch = false;
        }
    }
}
