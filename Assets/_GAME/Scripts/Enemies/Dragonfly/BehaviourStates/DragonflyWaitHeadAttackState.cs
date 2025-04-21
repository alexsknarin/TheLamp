using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Dragonfly.BehaviourStates
{
    public class DragonflyWaitHeadAttackState : IState
    {
        private const float ProximityThreshold = 0.25f;
        
        public bool IsReadyToSwitch;
        private Vector3 _targetPosition;
        private readonly Transform _transform;
        private readonly DragonflyPatrolAttackPositionProvider _patrolAttackPositionProvider;
        private readonly DragonflyMovement _movement;
        private float _prevDistance;

        public DragonflyWaitHeadAttackState(
            Transform visibleBodyTransform, 
            DragonflyPatrolAttackPositionProvider patrolAttackPositionProvider, 
            DragonflyMovement movement)
        {
            _transform = visibleBodyTransform;
            _patrolAttackPositionProvider = patrolAttackPositionProvider;
            _movement = movement;
        }
        
        public event Action Started;

        public void Enter()
        {
            IsReadyToSwitch = false;
            _targetPosition = _patrolAttackPositionProvider.GenerateRandomPreAttackHeadPosition(_movement.MovementState);
            _prevDistance = 0;
            Started?.Invoke();
        }

        public void Tick()
        {
            Vector3 currentPosition = GetCurrentNormalizedPosition();
            float distance = Vector3.Distance(currentPosition, _targetPosition);
            
            if (distance < ProximityThreshold && distance < _prevDistance)
            {
                _movement.StartAttack(PatrolAttackMode.Head);
                IsReadyToSwitch = true;
            }
            _prevDistance = distance;
        }

        private Vector3 GetCurrentNormalizedPosition()
        {
            Vector3 currentPosition = _transform.position;
            currentPosition.y = 0;
            currentPosition.Normalize();
            return currentPosition;
        }

        public void Exit()
        {
            IsReadyToSwitch = false;
        }
    }
}
