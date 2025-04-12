using UnityEngine;

namespace _GAME.Scripts.Enemies.Wasp.MovementStates
{
    public class WaspIdleState: WaspAnimBaseState
    {
        public WaspIdleState(Animator animator, int clipHash, Transform baseTransform) : 
            base(animator, clipHash, baseTransform) { }
    
        public override void Enter()
        {
            _animator.Play(_clipHash, -1, 0);
        }
    }
}
