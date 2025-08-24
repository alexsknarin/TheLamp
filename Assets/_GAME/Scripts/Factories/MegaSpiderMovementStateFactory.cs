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
        
        
        
        private Animator _animator;
        private Transform _visibleBodyTransform;
        private Transform _animatedTransform;
        
        // TODO: WILL BE LOCAL
        private bool _isLeftSide;
        private int _clipHash; 
        
        // TODO: Constructor (LATER)
        
        public void SetEnemyDependencies(
            Animator animator,
            Transform visibleBodyTransform,
            Transform animatedTransform
        )
        {
            _animator = animator;
            _visibleBodyTransform = visibleBodyTransform;
            _animatedTransform = animatedTransform;
        }

        public EnemyMovementStateBase Create(Type stateType)
        {
            if (stateType == typeof(MegaSpiderEnterLState))
            {
                return new MegaSpiderEnterLState(
                    _animator,
                    _enter,
                    _visibleBodyTransform,
                    _animatedTransform,
                    true
                    );
            }
            if (stateType == typeof(MegaSpiderEnterRState))
            {
                return new MegaSpiderEnterRState(
                    _animator,
                    _enter,
                    _visibleBodyTransform,
                    _animatedTransform,
                    false
                );
            }
            if (stateType == typeof(MegaSpiderZigzagAttackLState))
            {
                return new MegaSpiderZigzagAttackLState(
                    _animator,
                    _zigzagAttack,
                    _visibleBodyTransform,
                    _animatedTransform,
                    true
                );
            }
            if (stateType == typeof(MegaSpiderZigzagAttackRState))
            {
                return new MegaSpiderZigzagAttackRState(
                    _animator,
                    _zigzagAttack,
                    _visibleBodyTransform,
                    _animatedTransform,
                    false
                );
            }
            if (stateType == typeof(MegaSpiderProjectileBottomAttackLState))
            {
                return new MegaSpiderProjectileBottomAttackLState(
                    _animator,
                    _projectileBottomAttack,
                    _visibleBodyTransform,
                    _animatedTransform,
                    true
                );
            }
            if (stateType == typeof(MegaSpiderProjectileBottomAttackRState))
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
            
            return null;
        }
    }
}
