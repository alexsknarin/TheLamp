using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _GAME.Scripts.Enemies.Ladybug
{
    public class LadybugBodyRotationHandler : MonoBehaviour, IInitializable
    {
        private enum RotationState
        {
            Enter,
            PreAttack,
            Attack,
            Stick,
            Death,
            Idle
        }
        
        // TODO: Inject lamp position provider
        private const float EnterUpLowVelocityMix = 0.35f;
        [SerializeField] private Transform _bodyTransform;
        [SerializeField] private LadybugMovement _movement;
        [SerializeField] private RotationState _rotationState;
        [Header("Enter Settings")]
        [SerializeField] private float _enterForwardYMin = -0.5f;
        [SerializeField] private float _enterForwardYMax = 1.5f;
        [SerializeField] private AnimationCurve _enterDownForwardTargetMixCurve;
        [SerializeField] private AnimationCurve _enterDownForwardAngleMixCurve;
        [SerializeField] private Vector3 _enterForwardTargetFar;
        [SerializeField] private Vector3 _enterForwardTargetNear;
        [SerializeField] private float _enterForwardMaxDistanceToLamp;
        [SerializeField] private float _enterForwardMinDistanceToLamp;
        [SerializeField] private Vector3 _enterUpTarget;
        [Header("PreAttack Settings")] 
        [SerializeField] private float _preAttackDuration;
        [Header("Attack Settings")] 
        [SerializeField] private float _attackDuration;
        [Header("Death Settings")] 
        [SerializeField] private float _deathTransitionDuration;
        
        private Vector3 _previousPosition;
        private bool _isGoingUp;
        
        private float _localTime;
        
        private Vector3 _forward;
        private Vector3 _up;

        private Vector3 _preAttackForwardStartDirection;
        private Vector3 _preAttackForwardEndDirection;

        private Vector3 _attackForwardStartDirection;
        private Vector3 _attackForwardEndDirection;
        private Vector3 _attackUpStartDirection;
        private Vector3 _attackUpEndDirection;
        private float _attackEndAngle;

        public void Initialize()
        {
            _rotationState = RotationState.Enter;
            _movement.PreAttackStarted += OnPreattackStarted;
            _movement.PreAttackEnded += OnPreattackEnded;
            _movement.DeathStateStarted += OnDeathStateStarted;
        }

        private void OnDestroy()
        {
            _movement.PreAttackStarted -= OnPreattackStarted;
            _movement.PreAttackEnded -= OnPreattackEnded;
            _movement.DeathStateStarted += OnDeathStateStarted;
        }

        public void Play()
        {
            _rotationState = RotationState.Enter;
            _localTime = 0;
        }

        void Update()
        {
            switch (_rotationState)
            {
                case RotationState.Enter:
                    PerformEnterState();
                    break;
                case RotationState.PreAttack:
                    PerformPreAttackState();
                    break;
                case RotationState.Attack:
                    PerformAttackState();
                    break;
                case RotationState.Death:
                    PerformDeathState();
                    break;
            }
        
            Debug.DrawLine(transform.position, transform.position + _forward.normalized, Color.red);
            Debug.DrawLine(transform.position, transform.position + _up.normalized, Color.green);
        }

        private void PerformEnterState()
        {
            // Up Down Direction
            _isGoingUp = true;
            if (transform.position.y < _previousPosition.y)
            {
                _isGoingUp = false;
            }
        
            float yPhase = Mathf.InverseLerp(_enterForwardYMin, _enterForwardYMax, transform.position.y);
            // Forward
            Vector3 velocityForward = (transform.position - _previousPosition).normalized;
            Vector3 velocityTarget;
            // Moving Up
            if (_isGoingUp)
            {
                velocityTarget = (_enterForwardTargetFar - transform.position).normalized;
                float targetMix = Mathf.Lerp(EnterUpLowVelocityMix, 0f, yPhase);
                _forward = Vector3.Lerp(velocityForward, velocityTarget, targetMix);
            }
            else // Moving Down
            {
                Vector3 forwardTarget = Vector3.Lerp(
                    _enterForwardTargetFar, 
                    _enterForwardTargetNear, 
                    _enterDownForwardTargetMixCurve.Evaluate(yPhase));
                
                velocityTarget = (forwardTarget - transform.position).normalized;
                _forward = Vector3.Lerp(
                    velocityForward, 
                    velocityTarget, 
                    _enterDownForwardAngleMixCurve.Evaluate(yPhase));

            }

            float distanceToLamp = transform.position.magnitude;
            float lampDistanceMix = Mathf.InverseLerp(_enterForwardMaxDistanceToLamp, _enterForwardMinDistanceToLamp, distanceToLamp);
            _forward = Vector3.Lerp(_forward, -transform.position.normalized, lampDistanceMix).normalized;
        
            // Up1
            _up = _enterUpTarget - transform.position;
        
            _bodyTransform.LookAt(transform.position + _forward, _up);
            
            _previousPosition = transform.position;
        }

        private void PerformPreAttackState()
        {
            float phase = _localTime / _preAttackDuration;
            _forward = Vector3.LerpUnclamped(
                _preAttackForwardStartDirection,
                _preAttackForwardEndDirection, 
                phase);
            _up = transform.position.normalized;
            _bodyTransform.LookAt(transform.position + _forward, _up);

            _localTime += Time.deltaTime;
        }

        private void PerformAttackState()
        {
            // TODO: use distance as a phase instead of duration.
            // TODO: use real lamp position
            _up = transform.position.normalized;
            float phase = _localTime / _attackDuration;
            if (phase > 1f)
            {
                _rotationState = RotationState.Stick;
                _attackUpEndDirection = _up;
                return;
            }
            
            Vector3 crossVector = Vector3.Cross(transform.position.normalized, _attackForwardEndDirection);
            Debug.DrawLine(transform.position, transform.position + crossVector, Color.darkBlue);
            
            _attackForwardEndDirection = Vector3.Cross(crossVector, transform.position.normalized);
            Vector3 forwardEndDirectionRotated = Quaternion.AngleAxis(_attackEndAngle, _up) * _attackForwardEndDirection;
            
            _forward = Vector3.Lerp(_attackForwardStartDirection, forwardEndDirectionRotated, phase);
            
            _bodyTransform.LookAt(transform.position + _forward, _up);

            
            _localTime += Time.deltaTime;
        }

        private void PerformDeathState()
        {
            float phase = _localTime / _deathTransitionDuration;
            if (phase > 1f)
            {
                _rotationState = RotationState.Idle;
                return;
            }
            
            _up = Vector3.Lerp(_attackUpEndDirection, Vector3.down, phase);
            _bodyTransform.LookAt(transform.position + _forward, _up);
            _localTime += Time.deltaTime;
        }

        private void OnPreattackStarted()
        {
            _rotationState = RotationState.PreAttack;
            _localTime = 0;
            
            _preAttackForwardStartDirection = -transform.position.normalized;
            _preAttackForwardEndDirection = Vector3.back;
        }

        private void OnPreattackEnded()
        {
            _rotationState = RotationState.Attack;
            _localTime = 0;
            
            // Prepare Attack state
            _attackUpStartDirection = transform.position.normalized;
            _attackForwardStartDirection = _forward;
            Vector3 crossVector = Vector3.Cross(_attackUpStartDirection, _attackForwardStartDirection);
            _attackForwardEndDirection = Vector3.Cross(crossVector, _attackUpStartDirection);
            _attackEndAngle = Random.Range(-45f, 45f);
        }

        private void OnDeathStateStarted()
        {
            _rotationState = RotationState.Death;
            _localTime = 0;
        }
    }
}
