using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;
namespace _GAME.Scripts.Enemies.Dragonfly.BehaviourStates
{
    public class DragonflyPatrolTailState : IState
    {
        public bool ReadyToSwitch; // TODO: rename
        private readonly float _minWaitTime;
        private readonly float _maxWaitTime;
        private float _localTime;
        private float _duration;

        public DragonflyPatrolTailState(float minWaitTime, float maxWaitTime)
        {
            _minWaitTime = minWaitTime;
            _maxWaitTime = maxWaitTime;
        }

        public event Action Ended;

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
                Ended?.Invoke();
                ReadyToSwitch = true;
            }
        }

        public void Exit()
        {
            ReadyToSwitch = false;
        }
    }
}
