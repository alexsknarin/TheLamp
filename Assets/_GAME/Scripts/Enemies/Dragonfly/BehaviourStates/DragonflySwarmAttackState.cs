using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Dragonfly.BehaviourStates
{
    public class DragonflySwarmAttackState : IState
    {
        private readonly float _duration;
        private float _localTime = 0f;
        private bool _readyToSwitch = false;
        public bool ReadyToSwitch => _readyToSwitch;
    
        public DragonflySwarmAttackState(float duration)
        {
            _duration = duration;
        }
    
        public void Enter()
        {
            _localTime = 0f;
        }

        public void Tick()
        {
            _localTime += Time.deltaTime;
            if (_localTime >= _duration)
            {
                _readyToSwitch = true;
            }
        }

        public void Exit()
        {
            _readyToSwitch = false;
        }
    }
}
