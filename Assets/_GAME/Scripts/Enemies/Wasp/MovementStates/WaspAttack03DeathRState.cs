using System;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Wasp.MovementStates
{
    public class WaspAttack03DeathRState : FWaspAnimBaseState
    {
        public WaspAttack03DeathRState(Animator animator, int clipHash, Transform baseTransform) : 
            base(animator, clipHash, baseTransform) { }
    
        public event Action Ended;
    
        public override void OnEnter()
        {
            _baseTransform.localScale = _baseScaleR;
            _animator.Play(_clipHash, -1, 0);
        }
    
        public override void OnExit()
        {
            Ended?.Invoke();
        }
    }
}
