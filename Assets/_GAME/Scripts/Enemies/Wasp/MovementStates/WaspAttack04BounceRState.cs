using System;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Wasp.MovementStates
{
    public class WaspAttack04BounceRState : WaspAnimBaseState
    {
        public WaspAttack04BounceRState(Animator animator, int clipHash, Transform baseTransform) : 
            base(animator, clipHash, baseTransform) { }

        public event Action Started;
        
        public override void Enter()
        {
            _baseTransform.localScale = _baseScaleR;
            _animator.Play(_clipHash, -1, 0);
            Started?.Invoke();
        }
    }
}