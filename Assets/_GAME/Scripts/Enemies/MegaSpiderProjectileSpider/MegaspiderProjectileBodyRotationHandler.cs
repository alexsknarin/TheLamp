using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.MegaSpiderProjectileSpider
{
    public class MegaspiderProjectileBodyRotationHandler : MonoBehaviour, IInitializable
    {
        [SerializeField] private MegaspiderProjectileSpider.MegaspiderProjectileSpider _projectile;
        [SerializeField] private Transform _bodyTransform;
        private Vector3 _previousPosition;
        private Vector3 _velocityDirection;
        private Vector3 _forward;
        private Vector3 _up;
        private bool _isJumping = false;

        public void Initialize()
        {
            _projectile.JumpStarted += OnProjectileJumpStarted;
            _projectile.FallStarted += OnProjectileFallStarted;
        }

        private void OnDestroy()
        {
            _projectile.JumpStarted -= OnProjectileJumpStarted;
            _projectile.FallStarted -= OnProjectileFallStarted;
        }


        private void LateUpdate()
        {
            _velocityDirection = (_bodyTransform.position - _previousPosition).normalized;

            
            _forward = _velocityDirection;
            
            if (_isJumping)
                _up = Vector3.back;
            else
                _up = Vector3.up;
            
            
            _bodyTransform.LookAt(_bodyTransform.position + _forward, _up);
            _previousPosition = _bodyTransform.position;
        }

        private void OnProjectileJumpStarted()
        {
            _isJumping = true;
        }

        private void OnProjectileFallStarted()
        {
            _isJumping = false;
        }
    }
}
