using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;
namespace _GAME.Scripts.Enemies.Dragonfly.BehaviourStates
{
    public class DragonflyPatrolSpiderState : IState
    {
        public bool ReadyToSwitch;
        private readonly float _minWaitTime;
        private readonly float _maxWaitTime;
        private float _localTime;
        private float _duration;

        public DragonflyPatrolSpiderState(float minWaitTime, float maxWaitTime)
        {
            _minWaitTime = minWaitTime;
            _maxWaitTime = maxWaitTime;
        }

        public void Enter()
        {
            ReadyToSwitch = false;
            _localTime = 0;
            _duration = Random.Range(_minWaitTime, _maxWaitTime);
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
