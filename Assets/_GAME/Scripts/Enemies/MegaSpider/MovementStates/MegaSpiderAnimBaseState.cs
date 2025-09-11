using _GAME.Scripts.Lib;
using _GAME.Scripts.Lib.Interfaces;
using Unity.Hierarchy;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megaspider.MovementStates
{
    public class MegaspiderAnimBaseState: EnemyMovementStateBase
    {
        protected Animator _animator;
        protected int _clipHash;
        private Transform _visibleBodyTransform;
        private Transform _animatedTransform;
        private bool _isLeftSide;

        public MegaspiderAnimBaseState(
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
            HierarchyUtilities.ParentWithoutOffset(_visibleBodyTransform, _animatedTransform);
            _animator.enabled = true;
            _animator.Play(_clipHash, -1, 0);
        }

        public override void Tick() { }

        public override void Exit()
        {
            _animator.enabled = false;
            Debug.Log("Exiting Animation State");
        }
    }
}
