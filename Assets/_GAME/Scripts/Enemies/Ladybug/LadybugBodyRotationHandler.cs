using System;
using UnityEngine;

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
    
        [Header("up vector targets TMP")]
        [SerializeField] private Transform _upVectorTarget01;
        [SerializeField] private Transform _upVectorTarget02;
        [SerializeField] private Transform _upVectorTarget03;
    
        private Vector3 _previousPosition;
        private bool _isGoingUp;

        private bool _isEnter; // TODO: enum
        private bool _isPreAttacking;
        private bool _isAttacking;
        

        private void Awake()
        {
            _upVectorTarget01 = GameObject.Find("Target01").transform;
            _upVectorTarget02 = GameObject.Find("Target02").transform;
            _upVectorTarget03 = GameObject.Find("Target03").transform;
            
            
        _isEnter = true; // TODO: enum
        _isPreAttacking = false;
        _isAttacking = false;

        _movement.PreAttackStarted += OnPreattackStarted;
        }

        private void OnDestroy()
        {
            _movement.PreAttackStarted -= OnPreattackStarted;
        }

        void Update()
        {
            if (_isEnter)
                EnterState();

            if (_isPreAttacking)
                PreAttackState();

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
            Vector3 forward;
            // Moving Up
            if (_isGoingUp)
            {
                velocityTarget = (_enterForwardTargetFar - transform.position).normalized;
                float targetMix = Mathf.Lerp(EnterUpLowVelocityMix, 0f, yPhase);
                forward = Vector3.Lerp(velocityForward, velocityTarget, targetMix);
            }
            else // Moving Down
            {
                Vector3 forwardTarget = Vector3.Lerp(
                    _enterForwardTargetFar, 
                    _enterForwardTargetNear, 
                    _enterDownForwardTargetMixCurve.Evaluate(yPhase));
                
                velocityTarget = (forwardTarget - transform.position).normalized;
                forward = Vector3.Lerp(
                    velocityForward, 
                    velocityTarget, 
                    _enterDownForwardAngleMixCurve.Evaluate(yPhase));

            }

            float distanceToLamp = transform.position.magnitude;
            float lampDistanceMix = Mathf.InverseLerp(_enterForwardMaxDistanceToLamp, _enterForwardMinDistanceToLamp, distanceToLamp);
            forward = Vector3.Lerp(forward, -transform.position.normalized, lampDistanceMix).normalized;
            
            
        
            // Vector3 velocityForward = -transform.position.normalized;
        
            // Up1
            Vector3 velocityUp1 = _upVectorTarget01.position - transform.position;
            Vector3 velocityUp2 = _upVectorTarget02.position - transform.position;
            Vector3 velocityUp3 = _upVectorTarget03.position - transform.position;
        
        
        
            _bodyTransform.LookAt(transform.position + forward, velocityUp3);
        
        
            _previousPosition = transform.position;
        }

        private void PreAttackState()
        {
            Vector3 forward = Vector3.back;
            Vector3 up = transform.position.normalized;
            _bodyTransform.LookAt(transform.position + forward, up);
        }


        private void OnPreattackStarted()
        {
            _isEnter = false; // TODO: enum
            _isPreAttacking = true;
            _isAttacking = false;
        }

        private void OnDrawGizmos()
        {
            if (_isGoingUp)
            {
                Gizmos.color = Color.green;
            }
            else
            {
                Gizmos.color = Color.red;
            }

            Gizmos.DrawWireSphere(Vector3.zero, .75f);
        }
    }
}
