using System;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megaspider.MovementStates
{
    public class MegaspiderTangleAttackLState : MegaspiderTangleAttackBaseState
    {
        public MegaspiderTangleAttackLState(
            Transform lampTransform,
            Transform visibleBody,
            Transform calculatedTransform,
            AnimationCurve swingCurve,
            AnimationCurve dropCurve,
            bool isLeftSide
            ) : base(
                lampTransform, 
                visibleBody, 
                calculatedTransform, 
                swingCurve, 
                dropCurve, 
                isLeftSide
            )
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
