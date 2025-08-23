using UnityEngine;

namespace _GAME.Scripts.Enemies.MegaSpider.MovementStates
{
    public class MegaSpiderEnterRState : MegaSpiderAnimBaseState
    {
        public MegaSpiderEnterRState(
            Animator animator, 
            int clipHash, 
            Transform visibleBodyTransform, 
            Transform animatedTransform, 
            bool isLeftSide) 
            : base(
                animator, 
                clipHash, 
                visibleBodyTransform, 
                animatedTransform, 
                isLeftSide)
        {
        }
    }
}
