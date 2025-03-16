using UnityEngine;

namespace _GAME.Scripts.Enemies.Wasp.MovementStates
{
    public class WaspAttack01Success02RState : WaspAttack01RState
    {
        public WaspAttack01Success02RState(Animator animator, int clipHash, Transform baseTransform) : 
            base(animator, clipHash, baseTransform) { }
    
        public override void OnEnter()
        {
            _baseTransform.localScale = _baseScaleR;
            _animator.Play(_clipHash, -1, 0);
        }
    }
}
