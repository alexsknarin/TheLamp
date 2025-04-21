using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;
namespace _GAME.Scripts.Enemies.Dragonfly.BehaviourStates
{
    public class DragonflyPatrolTailState : IState
    {
        public bool IsReadyToSwitch;
        private readonly float _minWaitTime;
        private readonly float _maxWaitTime;
        private float _localTime;
        private float _duration;

        public DragonflyPatrolTailState(float minWaitTime, float maxWaitTime)
        {
            _minWaitTime = minWaitTime;
            _maxWaitTime = maxWaitTime;
        }

        public event Action Started;

        public void Enter()
        {
            IsReadyToSwitch = false;
            _localTime = 0;
            _duration = Random.Range(_minWaitTime, _maxWaitTime);
            Started?.Invoke();
        }

        public void Tick()
        {
            _localTime += Time.deltaTime;
            if (_localTime >= _duration)
            {
                IsReadyToSwitch = true;
            }
        }

        public void Exit()
        {
            IsReadyToSwitch = false;
        }
    }
}
