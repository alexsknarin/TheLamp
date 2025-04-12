using System;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Wasp.MovementStates
{
    public class WaspAttack04Success01RState : WaspAnimBaseState
    {
        public WaspAttack04Success01RState(Animator animator, int clipHash, Transform baseTransform) : 
            base(animator, clipHash, baseTransform) { }
    
        
        public event Action Ended;
        
        public override void Enter()
        {
            _baseTransform.localScale = _baseScaleR;
            _animator.Play(_clipHash, -1, 0);
        }
        
        public override void Exit()
        {
            Ended?.Invoke();
        }
    }
}