using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megabeetle
{
    public class MegabeetleBodyRotationHandler : MonoBehaviour, IInitializable
    {
        private enum RotationState
        {
            Enter,
            Attack,
            Stick,
            Fall,
            Death,
            Idle
        }
        
        private const float EnterUpLowVelocityMix = 0.35f;
        [SerializeField] private Transform _bodyTransform;
        [SerializeField] private MegabeetleMovement _movement;
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
        [SerializeField] private float _attackDuration; // TODO REMOVE???
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
        
        // Dependencies
        private ILampPositionProviderService _lampPositionProvider;
        
        public void Construct(ILampPositionProviderService lampPositionProvider)
        {
            _lampPositionProvider = lampPositionProvider;
        }

        public void Initialize()
        {
            _rotationState = RotationState.Enter;
            _movement.PreAttackEnded += OnPreAttackEnded;
            _movement.StickStarted += OnStickStarted;
            _movement.PatrolStarted += OnPatrolStarted;
        }

        private void OnDestroy()
        {
            _movement.PreAttackEnded -= OnPreAttackEnded;
            _movement.StickStarted -= OnStickStarted;
            _movement.PatrolStarted -= OnPatrolStarted;
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
                case RotationState.Attack:
                    PerformAttackState();
                    break;
                case RotationState.Stick:
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

        private void PerformAttackState()
        {
            // Forward
            _forward = ((Vector3)_lampPositionProvider.GetLampPosition() - transform.position).normalized;
            _bodyTransform.LookAt(transform.position + _forward, _up);
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


        private void OnPreAttackEnded()
        {
            _rotationState = RotationState.Attack;
            _up.z *= 0.25f; // TODO: MAGIC NUMBER
            _up.Normalize();
            _bodyTransform.LookAt(transform.position + _forward, _up);
        }

        private void OnStickStarted()
        {
            _rotationState = RotationState.Stick;
        }

        private void OnDeathStateStarted()
        {
            _rotationState = RotationState.Death;
            _localTime = 0;
        }

        private Vector3 GetCurrentDirectionFromLamp(Vector3 currentPosition)
        {
            return (currentPosition - (Vector3)_lampPositionProvider.GetLampPosition()).normalized;
        }

        private void OnPatrolStarted()
        {
            _rotationState = RotationState.Enter;
            _localTime = 0;
        }
    }
}
