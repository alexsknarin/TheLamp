using System;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Wasp.MovementStates
{
    public class WaspAttack02DeathLState : WaspAnimBaseState
    {
        public WaspAttack02DeathLState(Animator animator, int clipHash, Transform baseTransform) : 
            base(animator, clipHash, baseTransform) { }
    
        public event Action Started;
        public event Action Ended;
    
        public override void Enter()
        {
            _baseTransform.localScale = _baseScaleL;
            _animator.Play(_clipHash, -1, 0);
            
            Started?.Invoke();
        }
    
        public override void Exit()
        {
            Ended?.Invoke();
        }
    }
}

