using UnityEngine;

namespace _GAME.Scripts.Enemies.Megaspider.MovementStates
{
    public class MegaspiderProjectileDoubleUpAttackRState : MegaspiderAnimBaseState
    {
        public MegaspiderProjectileDoubleUpAttackRState(
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
