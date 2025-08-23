using System;
using _GAME.Scripts.Enemies;
using _GAME.Scripts.Enemies.Generic.States;
using _GAME.Scripts.Enemies.MegaSpider.MovementStates;
using UnityEngine;

namespace _GAME.Scripts.Factories
{
    public class MegaSpiderMovementStateFactory
    {
        private readonly int _enterL = Animator.StringToHash("MegaSpider_Enter01");
        
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
                    _enterL,
                    _visibleBodyTransform,
                    _animatedTransform,
                    true
                    );
            }
            return null;
        }
    }
}
