using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megamothling
{
    public class MegamothlingBodyRotationHandler : MonoBehaviour, IInitializable
    {
        private enum RotationState
        {
            Patrol,
            Attack,
            Fall
        }
    
        [SerializeField] private MegamothlingMovement _movement;
        [SerializeField] private Transform _bodyTransform;
        [SerializeField] private RotationState _rotationState;
        [SerializeField] private float _patrolSmoothTime = 0.35f;
    
        [SerializeField] private float _attackSmoothTime = 0.01f;
        [SerializeField] private float _attackSmoothTransitionDuration = 0.4f;
        [SerializeField] private float _fallSmoothTransitionDuration = 0.4f;
    
        private Vector3 _previousSmoothPosition = Vector3.zero;
        private Vector3 _currentSmoothPosition = Vector3.zero;
        private Vector3 _currentForwardVelocity;
        private Vector3 _smoothVelocity;
    
        private bool _isAttacking;
        private bool _isFalling;
    
        private float _smoothTransitionLocalTime;

        public void Initialize()
        {
            _movement.AttackStarted += OnAttackStarted;
            _movement.AttackEnded += OnAttackEnded;
        }

        private void OnDestroy()
        {
            _movement.AttackStarted -= OnAttackStarted;
            _movement.AttackEnded -= OnAttackEnded;
        }

        public void Play()
        {
            _rotationState = RotationState.Patrol;
        }

        void Update()
        {
            if (_rotationState == RotationState.Patrol)
            {
                _currentSmoothPosition = Vector3.SmoothDamp(_currentSmoothPosition, transform.position, ref _smoothVelocity, _patrolSmoothTime);
                Debug.DrawLine(_previousSmoothPosition, _currentSmoothPosition, Color.red, 5f);
                _currentForwardVelocity = (_currentSmoothPosition - _previousSmoothPosition).normalized;
            }
        
        
        
        
        
            Vector3 up = Vector3.up;
        
            _bodyTransform.LookAt(_bodyTransform.position + _currentForwardVelocity, up);
        
            _previousSmoothPosition = _currentSmoothPosition;
        
        
        
        
            // if (_isFalling)
            // {
            //     float fallSmoothTransitionPhase = _smoothTransitionLocalTime / _fallSmoothTransitionDuration;
            //
            //     if (fallSmoothTransitionPhase > 1f)
            //     {
            //         _isFalling = false;
            //     }
            //     
            //     _smoothTransitionLocalTime += Time.deltaTime;
            //     return;
            // }
            //
            //
            // // float currentSmoothTime = _smoothTime;
            //
            // if (_isAttacking)
            // {
            //     float smoothTransitionPhase = _smoothTransitionLocalTime / _attackSmoothTransitionDuration;
            //     if (smoothTransitionPhase > 1f)
            //     {
            //         currentSmoothTime = _attackSmoothTime;
            //     }
            //     else
            //     {
            //         currentSmoothTime = Mathf.Lerp(_smoothTime, _attackSmoothTime, smoothTransitionPhase);
            //         _smoothTransitionLocalTime += Time.deltaTime;
            //     }
            // }
            //
            // _currentSmoothPosition =
            //     Vector3.SmoothDamp(_currentSmoothPosition, transform.position, ref _smoothVelocity, currentSmoothTime);
            //
            // Debug.DrawLine(_previousSmoothPosition, _currentSmoothPosition, Color.red, 5f);
            //
            //
            // _currentForwardVelocity = (_currentSmoothPosition - _previousSmoothPosition).normalized;
            //
            //
            // Vector3 up = Vector3.up;
            // _bodyTransform.LookAt(_bodyTransform.position + _currentForwardVelocity, up);
            //
            //
            // _previousSmoothPosition = _currentSmoothPosition;
        }

        private void OnAttackStarted()
        {
            _isAttacking = true;
            _smoothTransitionLocalTime = 0f;
        }

        private void OnAttackEnded()
        {
            _isAttacking = false;
            _isFalling = true;
            _smoothTransitionLocalTime = 0f;
        }
    }
}
