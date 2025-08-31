using System;
using UnityEngine;

namespace _GAME.Scripts.Enemies.MegaSpider.MovementStates
{
    public class MegaSpiderHangJumpAttackLState : MegaSpiderAnimBaseState
    {
        public MegaSpiderHangJumpAttackLState(
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
        
        public event Action Started;

        public override void Enter()
        {
            base.Enter();
            Started?.Invoke();
        }
    }
}
