using System;
using _GAME.Scripts.Enemies;
using _GAME.Scripts.Enemies.Generic.States;
using _GAME.Scripts.Enemies.MegaSpider.MovementStates;
using UnityEngine;

namespace _GAME.Scripts.Factories
{
    public class MegaSpiderMovementStateFactory
    {
        private readonly int _enter = Animator.StringToHash("MegaSpider_Enter01");
        private readonly int _zigzagAttack = Animator.StringToHash("MegaSpider_ZigzagAttack01");
        private readonly int _projectileBottomAttack = Animator.StringToHash("MegaSpider_ProjectileBottomAttack01");
        private readonly int _projectileDoubleUpAttack = Animator.StringToHash("MegaSpider_ProjectileDoubleUpAttack01");
        private readonly int _hangAttack = Animator.StringToHash("MegaSpider_HangAttack01");
        private readonly int _hangJumpAttack = Animator.StringToHash("MegaSpider_HangJumpAttack01");
        private readonly int _projectileTopAttack = Animator.StringToHash("MegaSpider_ProjectileTopAttack01");
        private readonly int _projectileDoubleDownAttack = Animator.StringToHash("MegaSpider_ProjectileDoubleDownAttack01");
        
        
        // Dependencies
        private Animator _animator;
        private Transform _visibleBodyTransform;
        private Transform _animatedTransform;
        private Transform _calculatedTransform;
        private Transform _lampTransform;
        private AnimationCurve _swingCurve;
        private AnimationCurve _dropCurve;
        private Transform _cameraTransform;
        private float _bounceSpeed;
        private AnimationCurve _climbCurve;
        
        // TODO: WILL BE LOCAL
        private bool _isLeftSide;
        private int _clipHash; 
        
        // TODO: Constructor (LATER)
        
        public void SetEnemyDependencies(
            Animator animator,
            Transform visibleBodyTransform,
            Transform animatedTransform,
            Transform calculatedTransform,
            Transform lampTransform,
            AnimationCurve swingCurve,
            AnimationCurve dropCurve,
            Transform cameraTransform,
            float bounceSpeed, // TODO: move to config
            AnimationCurve climbCurve
        )
        {
            _animator = animator;
            _visibleBodyTransform = visibleBodyTransform;
            _animatedTransform = animatedTransform;
            _calculatedTransform = calculatedTransform;
            _lampTransform = lampTransform;
            _swingCurve = swingCurve;
            _dropCurve = dropCurve;
            _cameraTransform = cameraTransform;
            _bounceSpeed = bounceSpeed;
            _climbCurve = climbCurve;
        }

        public EnemyMovementStateBase Create(Type stateType)
        {
            if (stateType == typeof(MegaSpiderIdleState))
            {
                return new MegaSpiderIdleState(_visibleBodyTransform);
            }
            else if (stateType == typeof(MegaSpiderEnterLState))
            {
                return new MegaSpiderEnterLState(
                    _animator,
                    _enter,
                    _visibleBodyTransform,
                    _animatedTransform,
                    true
                    );
            }
            else if (stateType == typeof(MegaSpiderEnterRState))
            {
                return new MegaSpiderEnterRState(
                    _animator,
                    _enter,
                    _visibleBodyTransform,
                    _animatedTransform,
                    false
                );
            }
            else if (stateType == typeof(MegaSpiderZigzagAttackLState))
            {
                return new MegaSpiderZigzagAttackLState(
                    _animator,
                    _zigzagAttack,
                    _visibleBodyTransform,
                    _animatedTransform,
                    true
                );
            }
            else if (stateType == typeof(MegaSpiderZigzagAttackRState))
            {
                return new MegaSpiderZigzagAttackRState(
                    _animator,
                    _zigzagAttack,
                    _visibleBodyTransform,
                    _animatedTransform,
                    false
                );
            }
            else if (stateType == typeof(MegaSpiderProjectileBottomAttackLState))
            {
                return new MegaSpiderProjectileBottomAttackLState(
                    _animator,
                    _projectileBottomAttack,
                    _visibleBodyTransform,
                    _animatedTransform,
                    true
                );
            }
            else if (stateType == typeof(MegaSpiderProjectileBottomAttackRState))
            {
                return new MegaSpiderProjectileBottomAttackRState(
                    _animator,
                    _projectileBottomAttack,
                    _visibleBodyTransform,
                    _animatedTransform,
                    false
                );
            }
            if (stateType == typeof(MegaSpiderProjectileDoubleUpAttackLState))
            {
                return new MegaSpiderProjectileDoubleUpAttackLState(
                    _animator,
                    _projectileDoubleUpAttack,
                    _visibleBodyTransform,
                    _animatedTransform,
                    true
                );
            }
            if (stateType == typeof(MegaSpiderProjectileDoubleUpAttackRState))
            {
                return new MegaSpiderProjectileDoubleUpAttackRState(
                    _animator,
                    _projectileDoubleUpAttack,
                    _visibleBodyTransform,
                    _animatedTransform,
                    false
                );
            }
            if (stateType == typeof(MegaSpiderHangAttackLState))
            {
                return new MegaSpiderHangAttackLState(
                    _animator,
                    _hangAttack,
                    _visibleBodyTransform,
                    _animatedTransform,
                    true
                );
            }
            if (stateType == typeof(MegaSpiderHangAttackRState))
            {
                return new MegaSpiderHangAttackRState(
                    _animator,
                    _hangAttack,
                    _visibleBodyTransform,
                    _animatedTransform,
                    false
                );
            }
            if (stateType == typeof(MegaSpiderHangJumpAttackLState))
            {
                return new MegaSpiderHangJumpAttackLState(
                    _animator,
                    _hangJumpAttack,
                    _visibleBodyTransform,
                    _animatedTransform,
                    true
                );
            }
            if (stateType == typeof(MegaSpiderHangJumpAttackRState))
            {
                return new MegaSpiderHangJumpAttackRState(
                    _animator,
                    _hangJumpAttack,
                    _visibleBodyTransform,
                    _animatedTransform,
                    false
                );
            }
            if (stateType == typeof(MegaSpiderProjectileTopAttackLState))
            {
                return new MegaSpiderProjectileTopAttackLState(
                    _animator,
                    _projectileTopAttack,
                    _visibleBodyTransform,
                    _animatedTransform,
                    true
                );
            }
            if (stateType == typeof(MegaSpiderProjectileTopAttackRState))
            {
                return new MegaSpiderProjectileTopAttackRState(
                    _animator,
                    _projectileTopAttack,
                    _visibleBodyTransform,
                    _animatedTransform,
                    false
                );
            }
            if (stateType == typeof(MegaSpiderProjectileDoubleDownAttackLState))
            {
                return new MegaSpiderProjectileDoubleDownAttackLState(
                    _animator,
                    _projectileDoubleDownAttack,
                    _visibleBodyTransform,
                    _animatedTransform,
                    true
                );
            }
            if (stateType == typeof(MegaSpiderProjectileDoubleDownAttackRState))
            {
                return new MegaSpiderProjectileDoubleDownAttackRState(
                    _animator,
                    _projectileDoubleDownAttack,
                    _visibleBodyTransform,
                    _animatedTransform,
                    false
                );
            }
            
            if (stateType == typeof(MegaSpiderTangleAttackLState))
            {
                return new MegaSpiderTangleAttackLState(
                    _lampTransform,
                    _visibleBodyTransform,
                    _calculatedTransform,
                    _swingCurve,
                    _dropCurve,
                    true
                );
            }
            
            if (stateType == typeof(MegaSpiderTangleAttackRState))
            {
                return new MegaSpiderTangleAttackRState(
                    _lampTransform,
                    _visibleBodyTransform,
                    _calculatedTransform,
                    _swingCurve,
                    _dropCurve,
                    false
                    );
            }

            if (stateType == typeof(MegaSpiderWireAttackState))
            {
                return new MegaSpiderWireAttackState(
                    _visibleBodyTransform,
                    _calculatedTransform,
                    _lampTransform,
                    _cameraTransform
                    );
            }
            
            if (stateType == typeof(MegaSpiderBounceState))
            {
                return new MegaSpiderBounceState(
                    _visibleBodyTransform,
                    _calculatedTransform,
                    _lampTransform,
                    _bounceSpeed
                );
            }
            
            if (stateType == typeof(MegaSpiderFallState))
            {
                return new MegaSpiderFallState(
                    _visibleBodyTransform,
                    _calculatedTransform,
                    _lampTransform,
                    -3.5f, // TODO: use config
                    false
                    );
            }
            
            if (stateType == typeof(MegaSpiderSuccessFallState))
            {
                return new MegaSpiderSuccessFallState(
                    _visibleBodyTransform,
                    _calculatedTransform,
                    _lampTransform,
                    -1.6f, // TODO: use config
                    false
                );
            }
            
            if (stateType == typeof(MegaSpiderDropFallState))
            {
                return new MegaSpiderDropFallState(
                    _visibleBodyTransform,
                    _calculatedTransform,
                    _lampTransform,
                    -2.2f, // TODO: use config
                    true
                );
            }
            
            if (stateType == typeof(MegaSpiderSwingLState))
            {
                return new MegaSpiderSwingLState(
                    _visibleBodyTransform,
                    _calculatedTransform,
                    true
                );
            }
            
            if (stateType == typeof(MegaSpiderSwingRState))
            {
                return new MegaSpiderSwingRState(
                    _visibleBodyTransform,
                    _calculatedTransform,
                    false
                );
            }
            
            if (stateType == typeof(MegaSpiderClimbState))
            {
                return new MegaSpiderClimbState(
                    _visibleBodyTransform,
                    _calculatedTransform,
                    _climbCurve
                );
            }
            return null;
        }
    }
}
