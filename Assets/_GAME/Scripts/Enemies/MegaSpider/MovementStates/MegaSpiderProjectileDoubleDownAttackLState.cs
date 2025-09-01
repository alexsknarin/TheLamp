using UnityEngine;

namespace _GAME.Scripts.Enemies.Megaspider.MovementStates
{
    public class MegaspiderProjectileDoubleDownAttackLState : MegaspiderAnimBaseState
    {
        public MegaspiderProjectileDoubleDownAttackLState(
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
