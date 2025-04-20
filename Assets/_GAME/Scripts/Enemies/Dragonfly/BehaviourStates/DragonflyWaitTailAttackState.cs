using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Dragonfly.BehaviourStates
{
    public class DragonflyWaitTailAttackState : IState
    {
        private const float ProximityThreshold = 0.25f;
        public bool ReadyToSwitch; // TODO: rename according to other classes
        private Vector3 _targetPosition;
        private readonly Transform _transform;
        private readonly DragonflyPatrolAttackPositionProvider _patrolAttackPositionProvider;
        private readonly DragonflyMovement _movement;
        private float _prevDistance;

        public DragonflyWaitTailAttackState(Transform visibleBodyTransform, 
            DragonflyPatrolAttackPositionProvider patrolAttackPositionProvider, 
            DragonflyMovement movement)
        {
            _transform = visibleBodyTransform;
            _patrolAttackPositionProvider = patrolAttackPositionProvider;
            _movement = movement;
        }

        public void Enter()
        {
            ReadyToSwitch = false;
            _targetPosition = _patrolAttackPositionProvider.GenerateRandomPreAttackTailPosition(_movement.MovementState);
            _prevDistance = 0;
        }

        public void Tick()
        {
            Vector3 currentPosition = _transform.position;
            currentPosition.y = 0;
            currentPosition.Normalize();
            float distance = Vector3.Distance(currentPosition, _targetPosition);
           
            if (distance < ProximityThreshold && distance < _prevDistance) 
            {
                _movement.StartAttack(PatrolAttackMode.Tail);
                ReadyToSwitch = true;
            }
            _prevDistance = distance;
        }

        public void Exit()
        {
            ReadyToSwitch = false;
        }
    }
}
