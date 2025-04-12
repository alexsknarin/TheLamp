using System;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Wasp.MovementStates
{
    public class WaspAttack01Success02LState : WaspAnimBaseState
    {
        public WaspAttack01Success02LState(Animator animator, int clipHash, Transform baseTransform) : 
            base(animator, clipHash, baseTransform) { }
        
        public event Action Ended;
        public override void Enter()
        {
            _baseTransform.localScale = _baseScaleL;
            _animator.Play(_clipHash, -1, 0);
        }
        
        public override void Exit()
        {
            Ended?.Invoke();
        }
    }
}
