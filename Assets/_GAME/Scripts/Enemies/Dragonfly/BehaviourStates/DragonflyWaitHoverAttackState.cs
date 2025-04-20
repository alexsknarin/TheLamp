using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _GAME.Scripts.Enemies.Dragonfly.BehaviourStates
{
    public class DragonflyWaitHoverAttackState : IState
    {
        public bool ReadyToSwitch; // TODO: rename according to other classes
        private readonly float _minWaitTime;
        private readonly float _maxWaitTime;
        private readonly DragonflyMovement _movement;
        private float _localTime;
        private float _duration;

        public DragonflyWaitHoverAttackState(float minWaitTime, float maxWaitTime, DragonflyMovement movement)
        {
            _minWaitTime = minWaitTime;
            _maxWaitTime = maxWaitTime;
            _movement = movement;
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
                _movement.StartAttack(PatrolAttackMode.Head);
                ReadyToSwitch = true;
            }
        }

        public void Exit()
        {
            ReadyToSwitch = false;
        }
    }
}
