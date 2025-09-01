using System;
using _GAME.Scripts.Enemies;
using _GAME.Scripts.Enemies.Generic.States;
using _GAME.Scripts.Enemies.Megaspider.MovementStates;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Factories
{
    public class MegaspiderMovementStateFactory
    {
        private readonly int _enter = Animator.StringToHash("Megaspider_Enter01");
        private readonly int _zigzagAttack = Animator.StringToHash("Megaspider_ZigzagAttack01");
        private readonly int _projectileBottomAttack = Animator.StringToHash("Megaspider_ProjectileBottomAttack01");
        private readonly int _projectileDoubleUpAttack = Animator.StringToHash("Megaspider_ProjectileDoubleUpAttack01");
        private readonly int _hangAttack = Animator.StringToHash("Megaspider_HangAttack01");
        private readonly int _hangJumpAttack = Animator.StringToHash("Megaspider_HangJumpAttack01");
        private readonly int _projectileTopAttack = Animator.StringToHash("Megaspider_ProjectileTopAttack01");
        private readonly int _projectileDoubleDownAttack = Animator.StringToHash("Megaspider_ProjectileDoubleDownAttack01");
        
        
        // Dependencies
        private Transform _cameraTransform;
        private Transform _lampTransform;
        
        private Animator _animator;
        private Transform _visibleBodyTransform;
        private Transform _animatedTransform;
        private Transform _calculatedTransform;
        private AnimationCurve _swingCurve;
        private AnimationCurve _dropCurve;
        private AnimationCurve _climbCurve;
        private float _bounceSpeed;

        // TODO: WILL BE LOCAL
        private bool _isLeftSide;
        private int _clipHash; 
        
        // TODO: Constructor (LATER)
        public MegaspiderMovementStateFactory(
            Transform cameraTransform,
            Transform lampTransform
            )
        {
            _cameraTransform = cameraTransform;
            _lampTransform = lampTransform;
        }
        
        public void SetEnemyDependencies(
            Animator animator,
            Transform visibleBodyTransform,
            Transform animatedTransform,
            Transform calculatedTransform,
            AnimationCurve swingCurve,
            AnimationCurve dropCurve,
            float bounceSpeed, // TODO: move to config
            AnimationCurve climbCurve
        )
        {
            _animator = animator;
            _visibleBodyTransform = visibleBodyTransform;
            _animatedTransform = animatedTransform;
            _calculatedTransform = calculatedTransform;
            _swingCurve = swingCurve;
            _dropCurve = dropCurve;
            _bounceSpeed = bounceSpeed;
            _climbCurve = climbCurve;
        }

        public EnemyMovementStateBase Create(Type stateType)
        {
            if (stateType == typeof(MegaspiderIdleState))
            {
                return new MegaspiderIdleState(_visibleBodyTransform);
            }
            else if (stateType == typeof(MegaspiderEnterLState))
            {
                return new MegaspiderEnterLState(
                    _animator,
                    _enter,
                    _visibleBodyTransform,
                    _animatedTransform,
                    true
                    );
            }
            else if (stateType == typeof(MegaspiderEnterRState))
            {
                return new MegaspiderEnterRState(
                    _animator,
                    _enter,
                    _visibleBodyTransform,
                    _animatedTransform,
                    false
                );
            }
            else if (stateType == typeof(MegaspiderZigzagAttackLState))
            {
                return new MegaspiderZigzagAttackLState(
                    _animator,
                    _zigzagAttack,
                    _visibleBodyTransform,
                    _animatedTransform,
                    true
                );
            }
            else if (stateType == typeof(MegaspiderZigzagAttackRState))
            {
                return new MegaspiderZigzagAttackRState(
                    _animator,
                    _zigzagAttack,
                    _visibleBodyTransform,
                    _animatedTransform,
                    false
                );
            }
            else if (stateType == typeof(MegaspiderProjectileBottomAttackLState))
            {
                return new MegaspiderProjectileBottomAttackLState(
                    _animator,
                    _projectileBottomAttack,
                    _visibleBodyTransform,
                    _animatedTransform,
                    true
                );
            }
            else if (stateType == typeof(MegaspiderProjectileBottomAttackRState))
            {
                return new MegaspiderProjectileBottomAttackRState(
                    _animator,
                    _projectileBottomAttack,
                    _visibleBodyTransform,
                    _animatedTransform,
                    false
                );
            }
            if (stateType == typeof(MegaspiderProjectileDoubleUpAttackLState))
            {
                return new MegaspiderProjectileDoubleUpAttackLState(
                    _animator,
                    _projectileDoubleUpAttack,
                    _visibleBodyTransform,
                    _animatedTransform,
                    true
                );
            }
            if (stateType == typeof(MegaspiderProjectileDoubleUpAttackRState))
            {
                return new MegaspiderProjectileDoubleUpAttackRState(
                    _animator,
                    _projectileDoubleUpAttack,
                    _visibleBodyTransform,
                    _animatedTransform,
                    false
                );
            }
            if (stateType == typeof(MegaspiderHangAttackLState))
            {
                return new MegaspiderHangAttackLState(
                    _animator,
                    _hangAttack,
                    _visibleBodyTransform,
                    _animatedTransform,
                    true
                );
            }
            if (stateType == typeof(MegaspiderHangAttackRState))
            {
                return new MegaspiderHangAttackRState(
                    _animator,
                    _hangAttack,
                    _visibleBodyTransform,
                    _animatedTransform,
                    false
                );
            }
            if (stateType == typeof(MegaspiderHangJumpAttackLState))
            {
                return new MegaspiderHangJumpAttackLState(
                    _animator,
                    _hangJumpAttack,
                    _visibleBodyTransform,
                    _animatedTransform,
                    true
                );
            }
            if (stateType == typeof(MegaspiderHangJumpAttackRState))
            {
                return new MegaspiderHangJumpAttackRState(
                    _animator,
                    _hangJumpAttack,
                    _visibleBodyTransform,
                    _animatedTransform,
                    false
                );
            }
            if (stateType == typeof(MegaspiderProjectileTopAttackLState))
            {
                return new MegaspiderProjectileTopAttackLState(
                    _animator,
                    _projectileTopAttack,
                    _visibleBodyTransform,
                    _animatedTransform,
                    true
                );
            }
            if (stateType == typeof(MegaspiderProjectileTopAttackRState))
            {
                return new MegaspiderProjectileTopAttackRState(
                    _animator,
                    _projectileTopAttack,
                    _visibleBodyTransform,
                    _animatedTransform,
                    false
                );
            }
            if (stateType == typeof(MegaspiderProjectileDoubleDownAttackLState))
            {
                return new MegaspiderProjectileDoubleDownAttackLState(
                    _animator,
                    _projectileDoubleDownAttack,
                    _visibleBodyTransform,
                    _animatedTransform,
                    true
                );
            }
            if (stateType == typeof(MegaspiderProjectileDoubleDownAttackRState))
            {
                return new MegaspiderProjectileDoubleDownAttackRState(
                    _animator,
                    _projectileDoubleDownAttack,
                    _visibleBodyTransform,
                    _animatedTransform,
                    false
                );
            }
            
            if (stateType == typeof(MegaspiderTangleAttackLState))
            {
                return new MegaspiderTangleAttackLState(
                    _lampTransform,
                    _visibleBodyTransform,
                    _calculatedTransform,
                    _swingCurve,
                    _dropCurve,
                    true
                );
            }
            
            if (stateType == typeof(MegaspiderTangleAttackRState))
            {
                return new MegaspiderTangleAttackRState(
                    _lampTransform,
                    _visibleBodyTransform,
                    _calculatedTransform,
                    _swingCurve,
                    _dropCurve,
                    false
                    );
            }

            if (stateType == typeof(MegaspiderWireAttackState))
            {
                return new MegaspiderWireAttackState(
                    _visibleBodyTransform,
                    _calculatedTransform,
                    _lampTransform,
                    _cameraTransform
                    );
            }
            
            if (stateType == typeof(MegaspiderBounceState))
            {
                return new MegaspiderBounceState(
                    _visibleBodyTransform,
                    _calculatedTransform,
                    _lampTransform,
                    _bounceSpeed
                );
            }
            
            if (stateType == typeof(MegaspiderFallState))
            {
                return new MegaspiderFallState(
                    _visibleBodyTransform,
                    _calculatedTransform,
                    _lampTransform,
                    -3.5f, // TODO: use config
                    false
                    );
            }
            
            if (stateType == typeof(MegaspiderSuccessFallState))
            {
                return new MegaspiderSuccessFallState(
                    _visibleBodyTransform,
                    _calculatedTransform,
                    _lampTransform,
                    -1.6f, // TODO: use config
                    false
                );
            }
            
            if (stateType == typeof(MegaspiderDropFallState))
            {
                return new MegaspiderDropFallState(
                    _visibleBodyTransform,
                    _calculatedTransform,
                    _lampTransform,
                    -2.2f, // TODO: use config
                    true
                );
            }
            
            if (stateType == typeof(MegaspiderSwingLState))
            {
                return new MegaspiderSwingLState(
                    _visibleBodyTransform,
                    _calculatedTransform,
                    true
                );
            }
            
            if (stateType == typeof(MegaspiderSwingRState))
            {
                return new MegaspiderSwingRState(
                    _visibleBodyTransform,
                    _calculatedTransform,
                    false
                );
            }
            
            if (stateType == typeof(MegaspiderClimbState))
            {
                return new MegaspiderClimbState(
                    _visibleBodyTransform,
                    _calculatedTransform,
                    _climbCurve
                );
            }
            return null;
        }
    }
}
