using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _GAME.Scripts.Enemies.Dragonfly.BehaviourStates
{
    public class DragonflyWaitHoverAttackState : IState
    {
        public event Action<DragonflyPatrolAttackMode> Ended;
        private readonly float _minWaitTime = 0f;
        private readonly float _maxWaitTime = 1f;
        private float _localTime = 0f;
        private float _duration = 0f;
    
        public DragonflyWaitHoverAttackState(float minWaitTime, float maxWaitTime)
        {
            _minWaitTime = minWaitTime;
            _maxWaitTime = maxWaitTime;
        }

        public void Enter()
        {
            _localTime = 0;
            _duration = Random.Range(_minWaitTime, _maxWaitTime);
        }

        public void Tick()
        {
            _localTime += Time.deltaTime;
            if (_localTime >= _duration)
            {
                Ended?.Invoke(DragonflyPatrolAttackMode.Head);
            }
        }

        public void Exit() { }
    }
}
