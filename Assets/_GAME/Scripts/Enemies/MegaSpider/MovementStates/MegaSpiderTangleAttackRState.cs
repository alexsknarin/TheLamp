using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megaspider.MovementStates
{
    public class MegaspiderTangleAttackRState : MegaspiderTangleAttackBaseState
    {
        public MegaspiderTangleAttackRState(
            Transform lampTransform, 
            Transform visibleBody, 
            Transform calculatedTransform,  
            AnimationCurve swingCurve, 
            AnimationCurve dropCurve,
            IGameConfigService configService,
            float megaspiderRadius,
            bool isLeftSide
            ) : base(
                lampTransform, 
                visibleBody, 
                calculatedTransform, 
                swingCurve, 
                dropCurve,
                configService,
                megaspiderRadius,
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
