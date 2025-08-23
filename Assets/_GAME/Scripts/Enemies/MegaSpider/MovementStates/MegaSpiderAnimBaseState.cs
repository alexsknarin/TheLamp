using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.MegaSpider.MovementStates
{
    public class MegaSpiderAnimBaseState: EnemyMovementStateBase
    {
        protected Animator _animator;
        protected int _clipHash;
        private Transform _visibleBodyTransform;
        private Transform _animatedTransform;
        private bool _isLeftSide;

        public MegaSpiderAnimBaseState(
            Animator animator, 
            int clipHash, 
            Transform visibleBodyTransform, 
            Transform animatedTransform,
            bool isLeftSide)
        {
            _animator = animator;
            _clipHash = clipHash;
            _visibleBodyTransform = visibleBodyTransform;
            _animatedTransform = animatedTransform;
            _isLeftSide = isLeftSide;
        }
        
        public override void Enter()
        {
            Vector3 scale = Vector3.one;
            if (!_isLeftSide)
            {
                scale.x = -1;
            }
            _animatedTransform.parent.localScale = scale;
            ParentVisibleBodyToAnimatedTransform();
            _animator.Play(_clipHash, -1, 0);
        }

        public override void Tick() { }

        public override void Exit() { }

        private void ParentVisibleBodyToAnimatedTransform()
        {
            _visibleBodyTransform.SetParent(_animatedTransform, false);
            _visibleBodyTransform.localPosition = Vector3.zero;
            _visibleBodyTransform.localRotation = Quaternion.identity;
        }
    }
}
