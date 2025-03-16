using System;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Wasp.MovementStates
{
    public class WaspAttack02LState : FWaspAnimBaseState
    {
        public WaspAttack02LState(Animator animator, int clipHash, Transform baseTransform) : 
            base(animator, clipHash, baseTransform) { }
    
        public event Action Started;
    
        public override void OnEnter()
        {
            _baseTransform.localScale = _baseScaleL;
            _animator.Play(_clipHash, -1, 0);
            Started?.Invoke();
        }
    }
}

