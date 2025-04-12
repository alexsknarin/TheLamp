using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Dragonfly.BehaviourStates
{
    public class DragonflyWaitHeadAttackState : IState
    {
        private Vector3 _targetPosition;
        private readonly Transform _transform;
        private readonly DragonflyPatrolAttackPositionProvider _patrolAttackPositionProvider;
        private readonly DragonflyMovement _movement;
        private bool _isLastPatrolDirectionSet = false;
        private int _lastPatrolDirection = 0;

        public DragonflyWaitHeadAttackState(Transform visibleBodyTransform, 
            DragonflyPatrolAttackPositionProvider patrolAttackPositionProvider, 
            DragonflyMovement movement)
        {
            _transform = visibleBodyTransform;
            _patrolAttackPositionProvider = patrolAttackPositionProvider;
            _movement = movement;
        }

        public event Action<DragonflyPatrolAttackMode> Ended;

        public void Enter()
        {
            _targetPosition = _patrolAttackPositionProvider.GenerateRandomPreAttackHeadPosition(_movement.MovementState);
            _isLastPatrolDirectionSet = false;
            _lastPatrolDirection = 0;
        }

        public void Tick()
        {
            Vector3 currentPosition = _transform.position;
            currentPosition.y = 0;
            currentPosition.Normalize();
            float distance = Vector3.Distance(currentPosition, _targetPosition);
            if (distance < 0.25f)
            {
                if (!_isLastPatrolDirectionSet)
                {
                    _lastPatrolDirection = (int)Mathf.Sign((_targetPosition - currentPosition).normalized.x);
                    _isLastPatrolDirectionSet = true;
                }
                else
                {
                    float currentPatrolDirection = (int)Mathf.Sign((_targetPosition - currentPosition).normalized.x);
                    if (currentPatrolDirection + _lastPatrolDirection == 0)
                    {
                        Ended?.Invoke(DragonflyPatrolAttackMode.Head);
                    }
                }
            }
        }

        public void Exit() { }
    }
}
