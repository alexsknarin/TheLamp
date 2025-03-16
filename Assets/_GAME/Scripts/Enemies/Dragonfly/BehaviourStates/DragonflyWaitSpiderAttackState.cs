using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Dragonfly.BehaviourStates
{
    public class DragonflyWaitSpiderAttackState : IState
    {
        private Vector3 _targetPosition;
        private readonly Transform _transform;
        private readonly Vector3 _attackPositionBase;
        private bool _isLastPatrolDirectionSet = false;
        private int _lastPatrolDirection = 0;
        private float _preattackDistance = 0.4f;
        private bool _isReadyToPreAttack = false;

        public DragonflyWaitSpiderAttackState(Transform visibleBodyTransform, Vector3 attackPositionBase)
        {
            _transform = visibleBodyTransform;
            _attackPositionBase = attackPositionBase;
        }

        public event Action Ended;
        public event Action GotReadyToPreAttack;

        public void OnEnter()
        {
            _targetPosition = _attackPositionBase;
            _targetPosition.x *= RandomDirection.Generate();
            Debug.DrawRay(Vector3.zero, _targetPosition, Color.yellow, 5f);
            _targetPosition.y = 0;
            _targetPosition.Normalize();
        
            _isLastPatrolDirectionSet = false;
            _lastPatrolDirection = 0;
            _isReadyToPreAttack = false;
        }

        public void Tick()
        {
            Vector3 currentPosition = _transform.position;
            currentPosition.y = 0;
            currentPosition.Normalize();
            float distance = Vector3.Distance(currentPosition, _targetPosition);
            if (!_isReadyToPreAttack && distance < _preattackDistance)
            {
                _isReadyToPreAttack = true;
                GotReadyToPreAttack?.Invoke();
            }
        
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
                        Ended?.Invoke();
                    }
                }
            }
        }

        public void OnExit() { }
    }
}
