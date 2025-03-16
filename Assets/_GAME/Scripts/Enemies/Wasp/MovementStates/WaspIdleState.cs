using UnityEngine;

namespace _GAME.Scripts.Enemies.Wasp.MovementStates
{
    public class WaspIdleState: FWaspAnimBaseState
    {
        public WaspIdleState(Animator animator, int clipHash, Transform baseTransform) : 
            base(animator, clipHash, baseTransform) { }
    
        public override void OnEnter()
        {
            _animator.Play(_clipHash, -1, 0);
        }
    }
}
