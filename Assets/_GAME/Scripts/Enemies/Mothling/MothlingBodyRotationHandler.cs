using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Mothling
{
    public class MothlingBodyRotationHandler : MonoBehaviour, IInitializable
    {
        [SerializeField] private MothlingMovement _movement;
        [SerializeField] private Transform _bodyTransform;
        [SerializeField] private Vector3 _enterUpTarget;
        [SerializeField] private Vector3 _patrolUpTarget;
        [SerializeField] private float _enterPatrolTransitionDuration = 1f;
        [SerializeField] private bool _showGizmos = true;
        private Vector3 _currentUpTarget;
        private Vector3 _previousPosition;

        private bool _isTransitionMode;
        private float _localTime;

        private bool _isPreAttacking;

        public void Initialize()
        {
            _movement.EnterStateStarted += OnEnterStateStarted;
            _movement.EnterStateEnded += OnEnterStateEnded;
            _movement.DeathStateEnded += OnDeathStateEnded;
            _movement.PreAttackStarted += OnPreAttackStarted;
            _movement.PreAttackEnded += OnPreAttackEnded;
            
            Reset();
        }

        private void OnDestroy()
        {
            _movement.EnterStateStarted -= OnEnterStateStarted;
            _movement.EnterStateEnded -= OnEnterStateEnded;
            _movement.DeathStateEnded -= OnDeathStateEnded;
            _movement.PreAttackStarted -= OnPreAttackStarted;
            _movement.PreAttackEnded -= OnPreAttackEnded;
        }

        public void Reset()
        {
            _previousPosition = transform.position;
            _currentUpTarget = _enterUpTarget;
            _isTransitionMode = false;
        }

        private void OnPreAttackStarted()
        {
            _currentUpTarget = _enterUpTarget;
            _isPreAttacking = true;
        }

        private void OnPreAttackEnded()
        {
            _currentUpTarget = _patrolUpTarget;
            _isPreAttacking = false;
        }

        private void Update()
        {
            if (_isTransitionMode)
            {
                DoUpVectorTransition();
            }
            
            Vector3 forwardVelocity = Vector3.zero;
            if (_isPreAttacking)
            {
                forwardVelocity = -transform.position.normalized;
            }
            else
            {
                forwardVelocity = (transform.position - _previousPosition).normalized;    
            }
            
            _previousPosition = transform.position;
        
            Vector3 up = _currentUpTarget - transform.position;
            up.z *= -_movement.DepthSideDirection; 
            _bodyTransform.LookAt(_bodyTransform.position+ forwardVelocity, up);
        }

        private void OnEnterStateEnded()
        {
            _localTime = 0f;
            _isTransitionMode = true;
        }

        private void OnDeathStateEnded()
        {
            _currentUpTarget = _enterUpTarget;
        }

        private void DoUpVectorTransition()
        {
            float phase = _localTime / _enterPatrolTransitionDuration;
            _currentUpTarget = Vector3.Lerp(_enterUpTarget, _patrolUpTarget, phase);
            _localTime += Time.deltaTime;
            if (_localTime > _enterPatrolTransitionDuration)
            {
                _isTransitionMode = false;
                _localTime = 0f;
                _currentUpTarget = _patrolUpTarget;
            }
        }

        private void OnEnterStateStarted()
        {
            _currentUpTarget = _enterUpTarget;
        }

        private void OnDrawGizmos()
        {
            if (!_showGizmos) return;
            
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_enterUpTarget, 0.04f);

            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_patrolUpTarget, 0.04f);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, _currentUpTarget);
            
            Gizmos.color = Color.white;
        
        }
    }
}
