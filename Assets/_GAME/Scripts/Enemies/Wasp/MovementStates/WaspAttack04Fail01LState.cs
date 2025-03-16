using UnityEngine;

namespace _GAME.Scripts.Enemies.Wasp.MovementStates
{
    public class WaspAttack04Fail01LState : FWaspAnimBaseState
    {
        public WaspAttack04Fail01LState(Animator animator, int clipHash, Transform baseTransform) : 
            base(animator, clipHash, baseTransform) { }
    
        public override void OnEnter()
        {
            _baseTransform.localScale = _baseScaleL;
            _animator.Play(_clipHash, -1, 0);
        }
    }
}