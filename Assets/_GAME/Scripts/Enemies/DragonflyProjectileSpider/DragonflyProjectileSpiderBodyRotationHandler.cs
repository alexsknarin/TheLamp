using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.DragonflyProjectileSpider
{
    public class DragonflyProjectileSpiderBodyRotationHandler : MonoBehaviour, IInitializable
    {
        private enum RotationState
        {
            Enter,
            Curl,
            Attack,
            Fall
        }
        
        [SerializeField] private DragonflyProjectileSpiderMovement _movement;
        [SerializeField] private Transform _bodyTransform;
        [SerializeField] private RotationState _rotationState;
        [SerializeField] private float _curlRotationSpeed = 100f;
        [SerializeField] private float _fallRotationSpeed = 100f;
        
        private Vector3 _startPosition;
        private Vector3 _forward;
        private Vector3 _up;
        
        private int _fallRotationDirection = 1;
        
        private float _localTime;


        public void Initialize()
        {
            Debug.Log("SpiderProjectile Initialize");
            _movement.Started += OnStarted;
            _movement.EnterAnimationEnded += OnEnterAnimationEnded;
            _movement.FallStarted += OnFallStarted;
        }

        private void OnDestroy()
        {
            _movement.Started -= OnStarted;
            _movement.EnterAnimationEnded -= OnEnterAnimationEnded;
            _movement.FallStarted -= OnFallStarted;
        }

        private void LateUpdate()
        {
            if (_rotationState == RotationState.Enter)
            {
                _forward = (transform.position - _startPosition).normalized;
                _up = Vector3.back;
            }

            if (_rotationState == RotationState.Curl)
            {
                _up = transform.position.normalized;
                
                _forward = Vector3.up;
                Quaternion rotation = Quaternion.AngleAxis(_curlRotationSpeed * _localTime, _up);
                _forward = rotation * _forward;
                
                _localTime += Time.deltaTime;       
            }

            if (_rotationState == RotationState.Fall)
            {
                Quaternion rotation = Quaternion.AngleAxis(
                    _fallRotationSpeed * _localTime * _fallRotationDirection, Vector3.forward);
                
                _forward = rotation * _forward;
                _up = rotation * _up;
                
                _localTime += Time.deltaTime;       
            }

            _bodyTransform.LookAt(_bodyTransform.position + _forward, _up);
        }

        private void OnStarted(Vector3 startPosition)
        {
            _startPosition = startPosition;
            _rotationState = RotationState.Enter;
        }

        private void OnEnterAnimationEnded()
        {
            _rotationState = RotationState.Curl;
            _localTime = 0;
        }

        private void OnFallStarted()
        {
            _rotationState = RotationState.Fall;
            _localTime = 0;
            _fallRotationDirection = -(int)Mathf.Sign(transform.position.x);
        }
    }
}
