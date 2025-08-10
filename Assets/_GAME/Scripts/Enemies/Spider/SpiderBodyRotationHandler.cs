using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Spider
{
    public class SpiderBodyRotationHandler : MonoBehaviour, IInitializable
    {
        private enum RotationState
        {
            EnterPatrol,
            PreAttack,
            Attack,
            Damage,
            Death
        }
    
        [SerializeField] private Transform _bodyTransform;
        [SerializeField] private SpiderMovement _movement;
        [SerializeField] private RotationState _rotationState;
        [SerializeField] private float _height = 5f; // TODO: move to config
        [SerializeField] private float _xCenter;
        [Header("PreAttack - Attack state")]
        [SerializeField] private float _preAttackTransitionDuration;
        [SerializeField] private float _attackTransitionDuration;
        [SerializeField] private Vector3 _preAttackUpTarget;
        [SerializeField] private Vector3 _attackUpTargetL;
        [SerializeField] private Vector3 _attackUpTargetR;
        [Header("Damaged state")]
        [SerializeField] private float _damageTransitionDuration;
        [SerializeField] private AnimationCurve _damageRotationCurve;

        [Header("Death state")] 
        [SerializeField] private float _deathRotationSpeed;
    
        private Vector3 _hangingPoint = new (0, 0, 0);
        private Vector3 _currentUpTarget;
        private float _localTime;
    
        private Vector3 _forward;
        private Vector3 _up;
    
        public void Initialize()
        {
            _hangingPoint.x = _xCenter;
            _hangingPoint.y = _height;

            _movement.EnterStateStarted += OnEnterStateStarted;
            _movement.PreAttackStarted += PreAttackStarted;
            _movement.PreAttackEnded += OnPreAttackEnded;
            _movement.Bounced += OnBounced;
            _movement.DeathStateStarted += OnDeathStateStarted;
        }

        private void OnDestroy()
        {
            _movement.EnterStateStarted -= OnEnterStateStarted;
            _movement.PreAttackStarted -= PreAttackStarted;
            _movement.PreAttackEnded -= OnPreAttackEnded;
            _movement.Bounced -= OnBounced;
            _movement.DeathStateStarted -= OnDeathStateStarted;
        }


        private void LateUpdate()
        {
            switch (_rotationState)
            {
                case RotationState.EnterPatrol:
                    PerformEnterState();
                    break;
                case RotationState.PreAttack:
                    PerformPreAttackState();
                    break;
                case RotationState.Attack:
                    PerformAttackState();
                    break;
                case RotationState.Damage:
                    PerformDamageState();
                    break;
                case RotationState.Death:
                    PerformDeathState();
                    break;
            }
        
            _bodyTransform.LookAt(_bodyTransform.position + _forward, _up);
        }

        private void PerformEnterState()
        {
            // Forward
            _forward = (transform.position - _hangingPoint).normalized;
            // Up
            _up = Vector3.back;        
        }

        private void PerformPreAttackState()
        {
            // Forward
            _forward = (transform.position - _hangingPoint).normalized;
            // Up
            float phase = _localTime / _preAttackTransitionDuration;
            if (phase > 1)
            {
                _up = (_currentUpTarget - transform.position).normalized;
            }
            else
            {
                _up = Vector3.Lerp(Vector3.back, (_currentUpTarget - transform.position).normalized, phase).normalized;
                _localTime += Time.deltaTime;       
            }
        }

        private void PerformAttackState()
        {
            // Forward
            _forward = (transform.position - _hangingPoint).normalized;
        
            // Up
            float phase = _localTime / _attackTransitionDuration;
            if (phase > 1)
            {
                _up = (_currentUpTarget - transform.position).normalized;
            }
            else
            {
                _up = Vector3.Lerp(
                    (_preAttackUpTarget - transform.position).normalized, 
                    (_currentUpTarget - transform.position).normalized, phase).normalized;
                _localTime += Time.deltaTime;       
            }       
        }

        private void PerformDamageState()
        {
            // Forward
            _forward = (transform.position - _hangingPoint).normalized;
        
            // Up
            float phase = _localTime / _damageTransitionDuration;
            if (phase > 1)
            {
                _up = Vector3.back;
                _rotationState = RotationState.EnterPatrol;
            }
            else
            {
                Quaternion rotation = Quaternion.AngleAxis(_damageRotationCurve.Evaluate(phase), _forward);
                _up = rotation * Vector3.back;
                _localTime += Time.deltaTime;       
            }       
        }
    
        private void PerformDeathState()
        {
            // Forward
            Quaternion rotation = Quaternion.AngleAxis(_deathRotationSpeed * Time.deltaTime * _movement.SideDirection, _up);
            _forward = rotation * _forward;
        }

        private void OnEnterStateStarted()
        {
            _rotationState = RotationState.EnterPatrol;
            _hangingPoint.x = _xCenter * _movement.SideDirection;
        }

        private void PreAttackStarted()
        {
            _rotationState = RotationState.PreAttack;
            _currentUpTarget = _preAttackUpTarget;
            _localTime = 0;
        }

        private void OnPreAttackEnded()
        {
            _rotationState = RotationState.Attack;
            if (_movement.SideDirection == 1)
                _currentUpTarget = _attackUpTargetR;
            else
                _currentUpTarget = _attackUpTargetL;
            _localTime = 0;
        }

        private void OnBounced()
        {
            _rotationState = RotationState.Damage;
            _localTime = 0;
        }

        private void OnDeathStateStarted()
        {
            _rotationState = RotationState.Death;
            _localTime = 0;
        }
    }
}
