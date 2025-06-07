using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _GAME.Scripts.Enemies.Ladybug
{
    public class LadybugBodyRotationHandler : MonoBehaviour
    {
        // TODO: Inject lamp position provider
        private const float EnterUpLowVelocityMix = 0.35f;
        [SerializeField] private Transform _bodyTransform;
        [SerializeField] private LadybugMovement _movement;
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
        
        private Vector3 _previousPosition;
        private bool _isGoingUp;

        private bool _isEnter; // TODO: enum
        private bool _isPreAttacking;
        private bool _isAttacking;
        
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

        private void Awake()
        {
            _isEnter = true; // TODO: enum
            _isPreAttacking = false;
            _isAttacking = false;

            _movement.PreAttackStarted += OnPreattackStarted;
            _movement.PreAttackEnded += OnPreattackEnded;
            _movement.AttackEnded += OnAttackEnded;
        }

        private void OnDestroy()
        {
            _movement.PreAttackStarted -= OnPreattackStarted;
            _movement.PreAttackEnded -= OnPreattackEnded;
            _movement.AttackEnded -= OnAttackEnded;
        }

        void Update()
        {
            if (_isEnter)
                EnterState();

            if (_isPreAttacking)
                PreAttackState();
            
            if (_isAttacking)
                AttackState();
        
            Debug.DrawLine(transform.position, transform.position + _forward.normalized, Color.red);
            Debug.DrawLine(transform.position, transform.position + _up.normalized, Color.green);
        }

        private void EnterState()
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

        private void PreAttackState()
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

        private void AttackState()
        {
            // TODO: use distance as a phase instead of duration.
            // TODO: use real lamp position
            _up = transform.position.normalized;
            float phase = _localTime / _attackDuration;
            
            Vector3 crossVector = Vector3.Cross(transform.position.normalized, _attackForwardEndDirection);
            Debug.DrawLine(transform.position, transform.position + crossVector, Color.darkBlue);
            
            _attackForwardEndDirection = Vector3.Cross(crossVector, transform.position.normalized);
            Vector3 forwardEndDirectionRotated = Quaternion.AngleAxis(_attackEndAngle, _up) * _attackForwardEndDirection;
            
            _forward = Vector3.Lerp(_attackForwardStartDirection, forwardEndDirectionRotated, phase);
            
            _bodyTransform.LookAt(transform.position + _forward, _up);

            if (phase > 1f)
            {
                _isEnter = false; // TODO: enum
                _isPreAttacking = false;
                _isAttacking = false;
            }
            _localTime += Time.deltaTime;
        }

        private void OnPreattackStarted()
        {
            _isEnter = false; // TODO: enum
            _isPreAttacking = true;
            _isAttacking = false;

            _localTime = 0;
            
            _preAttackForwardStartDirection = -transform.position.normalized;
            _preAttackForwardEndDirection = Vector3.back;
        }

        private void OnPreattackEnded()
        {
            _isEnter = false; // TODO: enum
            _isPreAttacking = false;
            _isAttacking = true;

            _localTime = 0;
            
            // Prepare Attack state
            _attackUpStartDirection = transform.position.normalized;
            _attackForwardStartDirection = _forward;
            Vector3 crossVector = Vector3.Cross(_attackUpStartDirection, _attackForwardStartDirection);
            _attackForwardEndDirection = Vector3.Cross(crossVector, _attackUpStartDirection);
            _attackEndAngle = Random.Range(-45f, 45f);
        }

        private void OnAttackEnded()
        {
        }
    }
}
