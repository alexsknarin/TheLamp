using System;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megaspider.MovementStates
{
    public class MegaspiderZigzagAttackRState : MegaspiderAnimBaseState
    {
        public MegaspiderZigzagAttackRState(
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
