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
        [SerializeField] private Vector3 _forward;
        private Vector3 _up;
        [SerializeField] private Vector3 _bounceStartForwardDirection;
        [SerializeField] private Vector3 _hangPosition;
        [SerializeField] private float _bounceRotationDuration = 0.5f;
        private float _localTime;
        private Vector3 _swingPivot;
        private float _swingZoneSize;

        public void Construct(IGameConfigService configService)
        {
            _swingPivot = configService.GameConfig.MegaspiderSwingSwingPivot;
            _swingZoneSize = configService.GameConfig.MegaspiderSwingZoneSize;
        }

        public void Initialize()
        {
            _movement.EnterStateStarted += OnEnterStarted;
            _animationClipEvents.HangStartRequested += OnHangStartRequested;
            _movement.ZigzagAttackStateStarted += OnZigzagAttackStateStarted;
            _movement.WireAttackStateStarted += OnWireStateStarted;
            _movement.ProjectileAttackStateStarted += OnProjectileStateStarted;
            _movement.HangAttackStateStarted += OnHangAttackStateStarted;
            _movement.HangJumpAttackStateStarted += OnHangJumpAttackStateStarted;
            _movement.TangleAttackStarted += OnTangleAttackStarted;
            _movement.TanglePivotChanged += OnTanglePivotChanged;
            _movement.BounceStateStarted += OnBounceStateStarted;
            _movement.ClimbStateStarted += OnClimbStateStarted;
            _movement.SuccessFallOutForceCancelled += OnFallOutForceCancelled;
            _movement.FailFallOutForceCancelled += OnFallOutForceCancelled;
            _movement.FallStateEnded += OnFallOutForceCancelled;
        }

        private void OnDestroy()
        {
            _movement.EnterStateStarted -= OnEnterStarted;
            _animationClipEvents.HangStartRequested -= OnHangStartRequested;
            _movement.ZigzagAttackStateStarted -= OnZigzagAttackStateStarted;
            _movement.WireAttackStateStarted -= OnWireStateStarted;
            _movement.ProjectileAttackStateStarted -= OnProjectileStateStarted;
            _movement.HangAttackStateStarted -= OnHangAttackStateStarted;
            _movement.HangJumpAttackStateStarted -= OnHangJumpAttackStateStarted;
            _movement.TangleAttackStarted -= OnTangleAttackStarted;
            _movement.TanglePivotChanged -= OnTanglePivotChanged;
            _movement.BounceStateStarted -= OnBounceStateStarted;
            _movement.ClimbStateStarted -= OnClimbStateStarted;
            _movement.SuccessFallOutForceCancelled -= OnFallOutForceCancelled;
            _movement.FailFallOutForceCancelled -= OnFallOutForceCancelled;
            _movement.FallStateEnded -= OnFallOutForceCancelled;
        }

        private void LateUpdate()
        {
            _velocityDirection = (_bodyTransform.position - _previousPosition).normalized;
            switch (_rotationState)
            {
                case RotationState.Enter:
                    PerformEnterState();
                    break;
                case RotationState.ZigzagAttack:
                    PerformZigzagState();
                    break;
                case RotationState.WireAttack:
                    PerformWireState();
                    break;
                case RotationState.ProjectileAttack:
                    PerformProjectileState();
                    break;
                case RotationState.HangAttack:
                    PerformHangAttackState();
                    break;
                case RotationState.HangJumpAttack:
                    PerformHangJumpAttackState();
                    break;
                case RotationState.TangleAttack:
                    PerformTangleAttackState();
                    break;
                case RotationState.Bounce:
                    PerformBounceState();
                    break;
                case RotationState.Swing:
                    PerformSwingState();
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
            _hangPosition = _bodyTransform.position;
            _animationMode = AnimationMode.Climb;       
        }

        private void PerformEnterState()
        {
            if (_animationMode == AnimationMode.Run)
            {
                CalculateStandardRunVectors();    
            }
            else if (_animationMode == AnimationMode.Climb)
            {
                _forward = _velocityDirection;
                _up = Vector3.forward; // TODO: add slow rotation animation
            }
        }

        private void OnZigzagAttackStateStarted()
        {
            _rotationState = RotationState.ZigzagAttack;
            _animationMode = AnimationMode.Run;
        }

        private void PerformZigzagState()
        {
            CalculateStandardRunVectors();   
        }

        private void OnWireStateStarted()
        {
            _rotationState = RotationState.WireAttack;
            _animationMode = AnimationMode.Run;
        }

        private void PerformWireState()
        {
            CalculateStandardRunVectors();  
        }

        private void OnProjectileStateStarted()
        {
            _rotationState = RotationState.ProjectileAttack;
            _animationMode = AnimationMode.Run;
        }

        private void PerformProjectileState()
        {
            CalculateStandardRunVectors();    
        }

        private void OnHangAttackStateStarted()
        {
            _hangPosition = _bodyTransform.position;
            _rotationState = RotationState.HangAttack;
            _animationMode = AnimationMode.Climb;
        }

        private void PerformHangAttackState()
        {
            CalculateStandardHangVectors();
        }

        private void OnHangJumpAttackStateStarted()
        {
            _rotationState = RotationState.HangJumpAttack;
            _animationMode = AnimationMode.Run;
        }

        private void PerformHangJumpAttackState()
        {
            if (_animationMode == AnimationMode.Run)
            {
                CalculateStandardRunVectors();
            }
            else if (_animationMode == AnimationMode.Climb)
            {
                CalculateStandardHangVectors();    
            }
        }

        private void OnTangleAttackStarted(ITangledWireProvider wireProvider)
        {
            _hangPosition = wireProvider.StartPoint;
            
            _rotationState = RotationState.TangleAttack;
            _animationMode = AnimationMode.Climb;            
        }

        private void PerformTangleAttackState()
        {
            CalculateStandardHangVectors();   
        }

        private void OnTanglePivotChanged(Vector3 pivotPosition)
        {
            _hangPosition = pivotPosition;       
        }

        private void OnBounceStateStarted()
        {
            _bounceStartForwardDirection = _forward;
            _localTime = 0;
            _rotationState = RotationState.Bounce;
        }

        private void PerformBounceState()
        {
            if (_localTime < _bounceRotationDuration)
            {
                float phase = _localTime / _bounceRotationDuration;
                _forward = Vector3.LerpUnclamped(_bounceStartForwardDirection, Vector3.down, phase);
                _up = Vector3.back;    
            }
            _localTime += Time.deltaTime;
        }

        private void OnClimbStateStarted()
        {
            _rotationState = RotationState.Climb;
        }
        
        private void OnFallOutForceCancelled(IPositionProvider positionProvider)
        {
            if (Mathf.Abs(positionProvider.Position3D.x) > _swingZoneSize)
            {
                return;
            }
            
            _hangPosition = _swingPivot;
            _hangPosition.x = Mathf.Abs(_hangPosition.x) * Mathf.Sign(positionProvider.Position3D.x);
            _rotationState = RotationState.Swing;
        }
        
        private void PerformSwingState()
        {
            CalculateStandardHangVectors();
        }
        
        private void CalculateStandardRunVectors()
        {
            _forward = _velocityDirection;
            _up = (_enterUpTarget - _bodyTransform.position).normalized;
        }

        private void CalculateStandardHangVectors()
        {
            _forward = -(_hangPosition - _bodyTransform.position).normalized;
            _up = Vector3.back;
        }
    }
}
