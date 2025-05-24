using UnityEngine;

namespace _GAME.Scripts.Enemies.Fly
{
    public class FlyBodyRotationHandler : MonoBehaviour
    {
        [SerializeField] private FlyMovement _movement;
        [SerializeField] private Transform _bodyTransform;
        [SerializeField] private Vector3 _enterUpTarget;
        [Range(0f, 1f)]
        [SerializeField] private float _forwardVelocityNoiseMix = 0f;
        [Header("Patrol UP")]
        [SerializeField] private Vector3 _patrolUpTargetPositiveMin;
        [SerializeField] private Vector3 _patrolUpTargetPositiveMax;
        [SerializeField] private Vector3 _patrolUpTargetNegativeMin;
        [SerializeField] private Vector3 _patrolUpTargetNegativeMax;
        [SerializeField] private float _patrolPositiveMinY;
        [SerializeField] private float _patrolPositiveMaxY;
        [SerializeField] private float _patrolNegativeMinY;
        [SerializeField] private float _patrolNegativeMaxY;
        [SerializeField] private float _forwardVelocitySmoothTime = 0.7f;
        [Header("Attack UP")]
        [SerializeField] private Vector3 _attackUpTarget;
        [Header(" ")]
        [SerializeField] private bool _showGizmos = true;
        private Vector3 _currentUpTarget;
        private Vector3 _previousPosition;
        
        private Vector3 _currentForwardVelocity;
       

        private float _patrolUpPositiveAmplitude; 
        private float _patrolUpNegativeAmplitude; 

        private bool _isAttacking;

        public void Initialize()
        {
            _movement.PreAttackStarted += OnPreAttackStarted;
            _movement.AttackEnded += OnAttackEnded;
            
            Reset();
        }

        private void OnDestroy()
        {
            _movement.PreAttackStarted -= OnPreAttackStarted;
            _movement.AttackEnded -= OnAttackEnded;
        }

        public void Reset()
        {
            _patrolUpPositiveAmplitude = _patrolPositiveMaxY - _patrolPositiveMinY; 
            _patrolUpNegativeAmplitude = _patrolNegativeMaxY - _patrolNegativeMinY;
        }

        private void Update()
        {
            Vector3 currentPosition = Vector3.Lerp(_movement.Position3DRaw, transform.position, _forwardVelocityNoiseMix);
            Vector3 previousPosition = Vector3.Lerp(_movement.PreviousPosition3DRaw, _previousPosition, _forwardVelocityNoiseMix);
            
            if (_isAttacking)
            {
                _currentForwardVelocity = -transform.position.normalized;
                _currentUpTarget = _attackUpTarget;
            }
            else
            {
                _currentForwardVelocity = (currentPosition - previousPosition).normalized;
                _currentUpTarget = GetPatrolUpVectorTarget();    
            }
            
            Vector3 up = _currentUpTarget - transform.position;
            _bodyTransform.LookAt(_bodyTransform.position + _currentForwardVelocity, up);
            _previousPosition = transform.position;
        }

        private Vector3 GetPatrolUpVectorTarget()
        {
            Vector3 upTarget = Vector3.zero;
            if (_movement.DepthSideDirection == 1)
            {
                float yPhase = (transform.position.y - _patrolPositiveMinY) / _patrolUpPositiveAmplitude;
                upTarget = Vector3.Lerp(_patrolUpTargetPositiveMin, _patrolUpTargetPositiveMax, yPhase);
            }
            else
            {
                float yPhase = (transform.position.y - _patrolNegativeMinY) / _patrolUpNegativeAmplitude;
                upTarget = Vector3.Lerp(_patrolUpTargetNegativeMin, _patrolUpTargetNegativeMax, yPhase);
            }
            
            return upTarget;
        }

        private void OnPreAttackStarted()
        {
            _isAttacking = true;
        }

        private void OnAttackEnded()
        {
            _isAttacking = false;
        }

        private void OnDrawGizmos()
        {
            if (!_showGizmos) return;
            
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(_currentUpTarget, 0.04f);
            
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, _currentUpTarget);
            
            Gizmos.color = Color.white;
        
        }
    }
}
