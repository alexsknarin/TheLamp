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
            if (stateType == typeof(MegaSpiderProjectileBottomLAttackState))
            {
                return new MegaSpiderProjectileBottomLAttackState(
                    _animator,
                    _projectileBottomAttack,
                    _visibleBodyTransform,
                    _animatedTransform,
                    true
                );
            }
            if (stateType == typeof(MegaSpiderProjectileBottomRAttackState))
            {
                return new MegaSpiderProjectileBottomRAttackState(
                    _animator,
                    _projectileBottomAttack,
                    _visibleBodyTransform,
                    _animatedTransform,
                    false
                );
            }
            return null;
        }
    }
}
