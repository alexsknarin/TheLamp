using System;
using _GAME.Scripts.Enemies.Megaspider;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.MegaSpider
{
    public class MegaspiderBodyRotationHandler : MonoBehaviour, IInitializable
    {
        private enum RotationState
        {
            Idle,
            Enter,
            ZigzagAttack,
            ProjectileAttack,
            HangAttack,
            HangJumpAttack,
            TangleAttack,
            WireAttack,
            Bounce,
            SuccessFall,
            FailFall,
            DeathFall,
            Climb,
            Swing
        }
        
        private enum AnimationMode
        {
            Run,
            Climb,
            Swing
        }
        
        [SerializeField] private Transform _bodyTransform;
        [SerializeField] private MegaspiderMovement _movement;
        [SerializeField] private RotationState _rotationState = RotationState.Idle;
        [SerializeField] private AnimationMode _animationMode = AnimationMode.Run;
        [SerializeField] private MegaspiderAnimationClipEventListener _animationClipEvents;
        [SerializeField] private Vector3 _enterUpTarget;
        private Vector3 _previousPosition;
        private Vector3 _velocityDirection;
        private Vector3 _forward;
        private Vector3 _up;

        public void Initialize()
        {
            _movement.EnterStateStarted += OnEnterStarted;
            _animationClipEvents.HangStartRequested += OnHangStartRequested;
        }

        private void OnDestroy()
        {
            _movement.EnterStateStarted -= OnEnterStarted;
            _animationClipEvents.HangStartRequested -= OnHangStartRequested;
        }

        private void LateUpdate()
        {
            _velocityDirection = (_bodyTransform.position - _previousPosition).normalized;
            switch (_rotationState)
            {
                case RotationState.Enter:
                    PerformEnterState();
                    break;
            }
        
            _bodyTransform.LookAt(_bodyTransform.position + _forward, _up);
            _previousPosition = _bodyTransform.position;
        }


        private void OnEnterStarted()
        {
            _rotationState = RotationState.Enter;
            _animationMode = AnimationMode.Run;
        }

        private void OnHangStartRequested()
        {
            _animationMode = AnimationMode.Climb;       
        }

        private void PerformEnterState()
        {
            if (_animationMode == AnimationMode.Run)
            {
                _forward = _velocityDirection;
                _up = (_enterUpTarget - _bodyTransform.position).normalized;    
            }
            else if (_animationMode == AnimationMode.Climb)
            {
                _forward = _velocityDirection;
                _up = Vector3.forward; // TODO: add slow rotation animation
            }
        }
    }
}
