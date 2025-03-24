using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Dragonfly.FMovementStates
{
    public abstract class FDragonflyAnimBaseState : ScriptableObject, IState
    {
        protected Animator _animator;
        protected int _clipHash;
        // Dependencies
        private Transform _visibleBodyTransform;
        private Transform _animatedTransform;

        public void SetDependencies(Transform visibleBodyTransform, Transform animatedTransform, Animator animator, int clipHash)
        {
            _visibleBodyTransform = visibleBodyTransform;
            _animatedTransform = animatedTransform;
            _animator = animator;
            _clipHash = clipHash;
        }
    
        protected void ParentVisibleBodyToAnimatedTransform()
        {
            _visibleBodyTransform.SetParent(_animatedTransform, false);
            _visibleBodyTransform.localPosition = Vector3.zero;
            _visibleBodyTransform.localRotation = Quaternion.identity;
        }

        public virtual void OnEnter()
        {
            ParentVisibleBodyToAnimatedTransform();
            _animator.Play(_clipHash, -1, 0);
        }

        public void Tick() { }

        public void OnExit() { }
    }
}
