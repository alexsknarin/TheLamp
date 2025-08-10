using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.DragonflyProjectileMoth
{
    public class DragonflyMothBodyRotationHandler : MonoBehaviour, IInitializable
    {
        private enum RotationState
        {
            Attack,
            Fall,
            Flee
        }
        
        [SerializeField] private DragonflyProjectileMovementMoth _movement;
        [SerializeField] private Transform _bodyTransform;
        [SerializeField] private float _fallRotationDuration = 0.5f;
        private RotationState _rotationState = RotationState.Attack;
        private Vector3 _fleeDirection;
        private float _localTime;

        public void Initialize()
        {
            _movement.AttackStarted += OnAttackStarted;
            _movement.FallStarted += OnFallStarted;
            _movement.FleeStarted += OnFleeStarted;
        }

        private void OnDestroy()
        {
            _movement.AttackStarted -= OnAttackStarted;
            _movement.FallStarted -= OnFallStarted;
            _movement.FleeStarted -= OnFleeStarted;
        }


        private void Update()
        {
            if (_rotationState == RotationState.Attack)
            {
                _bodyTransform.LookAt(transform.position - transform.position * 2f, Vector3.up);
            }
            
            if (_rotationState == RotationState.Fall)
            {
                float phase = _localTime / _fallRotationDuration;
                Vector3 rotationDirection = Vector3.Lerp(transform.position.normalized, Vector3.down, phase);
                _bodyTransform.LookAt(rotationDirection, transform.position.normalized);
                _localTime += Time.deltaTime;
            }

            if (_rotationState == RotationState.Flee)
            {
                _bodyTransform.LookAt(transform.position + _fleeDirection, Vector3.up);
            }
        }

        private void OnAttackStarted()
        {
            _rotationState = RotationState.Attack;
        }

        private void OnFallStarted()
        {
            _rotationState = RotationState.Fall;
            _localTime = 0;
        }

        private void OnFleeStarted(Vector3 direction)
        {
            _fleeDirection = direction;
            _rotationState = RotationState.Flee;
        }
    }
}
