using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Moth
{
    public class MothBodyRotationHandler : MonoBehaviour, IInitializable
    {
        [SerializeField] private Transform _bodyTransform;
        [SerializeField] private MothMovement _mothMovement; 
        [SerializeField] private float _upTargetYMin;
        [SerializeField] private float _upTargetYMax;
        [Header("Forward Target")]
        [SerializeField] private Vector3 _forwardDirectionTargetMin;
        [SerializeField] private Vector3 _forwardDirectionTargetMax;
        [Header("Up Target")]
        [SerializeField] private Vector3 _upTargetMin;
        [SerializeField] private Vector3 _upTargetMax;
        [Header("Move To Hover")]
        [SerializeField] private float _transitionDuration = 0.5f;
        
        private Vector3 _previousPosition;
        private float _localTime;
        private bool _isMovingToHover;

        public void Initialize()
        {
            _mothMovement.PatrolStateStarted += OnPatrolStateStarted;
            _mothMovement.HoverStateStarted += OnHoverStateStarted;
        }

        private void OnDestroy()
        {
            _mothMovement.PatrolStateStarted -= OnPatrolStateStarted;
            _mothMovement.HoverStateStarted -= OnHoverStateStarted;
        }

        void Update()
        {
            float yPhase = Mathf.InverseLerp(_upTargetYMin, _upTargetYMax, transform.position.y);

            Vector3 forwardTarget = Vector3.Lerp(_forwardDirectionTargetMin, _forwardDirectionTargetMax, yPhase);
            Vector3 upTarget = Vector3.Lerp(_upTargetMin, _upTargetMax, yPhase);    
            
            Vector3 forwardDirection = (forwardTarget - transform.position).normalized;
            Vector3 up = (upTarget - transform.position).normalized;

            if (_isMovingToHover)
            {
                float phase = _localTime / _transitionDuration;
                Vector3 velocityForward = (transform.position - _previousPosition).normalized;
                
                if (phase > 1)
                {
                    forwardDirection = velocityForward;
                }
                else
                {
                    
                    forwardDirection = Vector3.Lerp(forwardDirection, velocityForward, phase);
                }
                _localTime += Time.deltaTime;
            }
        
            _bodyTransform.LookAt(transform.position + forwardDirection, up);
            
            _previousPosition = transform.position;
        }

        private void OnHoverStateStarted()
        {
            _isMovingToHover = false;
        }

        private void OnPatrolStateStarted()
        {
            _isMovingToHover = true;
            _localTime = 0;
        }
    }
}
