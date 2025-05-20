using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megamothling
{
    public class MegamothlingBodyRotationHandler : MonoBehaviour, IInitializable
    {
        private enum RotationState
        {
            Enter,
            Patrol,
            PreAttack,
            Attack,
            Fall
        }
    
        [SerializeField] private MegamothlingMovement _movement;
        [SerializeField] private Transform _bodyTransform;
        [SerializeField] private RotationState _rotationState;
        [SerializeField] private float _patrolSmoothTime = 0.35f;
        [SerializeField] private float _preAttackDuration = 0.51f;
        
        [Header("Up Vector settings")]
        [SerializeField] private Vector3 _enterUpTarget;
        [SerializeField] private float _enterTransitionDuration = 0.5f;
        [SerializeField] private Vector3 _patrolUpTargetMin;
        [SerializeField] private Vector3 _patrolUpTargetMax;
        [SerializeField] private float _patrolUpYPositionMin;
        [SerializeField] private float _patrolUpYPositionMax;
   
        private Vector3 _previousSmoothPosition = Vector3.zero;
        private Vector3 _currentSmoothPosition = Vector3.zero;
        private Vector3 _currentForwardVelocity;
        private Vector3 _smoothVelocity;
        
        private Vector3 _lastForwardVelocity;
        private Vector3 _attackForwardVelocity;
        private float _localTime;
        private Vector3 _up;

        public void Initialize()
        {
            _movement.PreAttackStarted += OnPreattackStarted;
            _movement.AttackStarted += OnAttackStarted;
            _movement.AttackEnded += OnAttackEnded;
        }

        private void OnDestroy()
        {
            _movement.PreAttackStarted -= OnPreattackStarted;
            _movement.AttackStarted -= OnAttackStarted;
            _movement.AttackEnded -= OnAttackEnded;
        }

        public void Play()
        {
            _rotationState = RotationState.Enter;
            _localTime = 0;
        }

        void Update()
        {
            if (_rotationState == RotationState.Enter)
            {
                float enterPhase = _localTime / _enterTransitionDuration;
                
                if (enterPhase > 1)
                {
                    _rotationState = RotationState.Patrol;
                    _localTime = 0;
                }
                else
                {
                    PatrolForwardVectorUpdate();
                    _up = Vector3.Lerp((_enterUpTarget - transform.position).normalized, _patrolUpTargetMin, enterPhase);
                    _localTime += Time.deltaTime;
                }
            }
            
            if (_rotationState == RotationState.Patrol)
            {
                PatrolForwardVectorUpdate();
                
                float yPhase = Mathf.InverseLerp(_patrolUpYPositionMin, _patrolUpYPositionMax, transform.position.y);
                Vector3 upTarget = Vector3.Lerp(_patrolUpTargetMin, _patrolUpTargetMax, yPhase);
                
                _up = (upTarget - transform.position).normalized;
            }

            if (_rotationState == RotationState.PreAttack)
            {
                PreattackForwardVelocityUpdate();
            }

            if (_rotationState == RotationState.Attack)
            {
                _currentForwardVelocity = _attackForwardVelocity;
            }
        
        
            _bodyTransform.LookAt(_bodyTransform.position + _currentForwardVelocity, _up);
        
            _previousSmoothPosition = _currentSmoothPosition;
        
        }

        private void PatrolForwardVectorUpdate()
        {
            _currentSmoothPosition = Vector3.SmoothDamp(_currentSmoothPosition, transform.position, ref _smoothVelocity, _patrolSmoothTime);
            Debug.DrawLine(_previousSmoothPosition, _currentSmoothPosition, Color.red, 5f);
            _currentForwardVelocity = (_currentSmoothPosition - _previousSmoothPosition).normalized;
        }

        private void PreattackForwardVelocityUpdate()
        {
            float preAttackPhase = _localTime / _preAttackDuration;

            if (preAttackPhase > 1)
            {
                _rotationState = RotationState.Attack;
                                        
            }
            else
            {
                Vector3 currentSmoothPosition = Vector3.SmoothDamp(_currentSmoothPosition, transform.position, ref _smoothVelocity, _patrolSmoothTime);
                Debug.DrawLine(_previousSmoothPosition, currentSmoothPosition, Color.red, 5f);
                    
                Vector3 currentForwardVelocity = (_currentSmoothPosition - _previousSmoothPosition).normalized;
                _currentForwardVelocity = Vector3.Slerp(_lastForwardVelocity, _attackForwardVelocity, preAttackPhase);
                    
                _localTime += Time.deltaTime;
            }
        }

        private void OnPreattackStarted()
        {
            _lastForwardVelocity = _currentForwardVelocity;
            _attackForwardVelocity = -transform.position.normalized;
            _localTime = 0;
            _rotationState = RotationState.PreAttack;
        }

        private void OnAttackStarted()
        {
            // _isAttacking = true;
            // _smoothTransitionLocalTime = 0f;
        }

        private void OnAttackEnded()
        {
            _rotationState = RotationState.Patrol;
        }
    }
}
