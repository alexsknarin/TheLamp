using UnityEngine;

namespace _GAME.Scripts.Enemies.MegaSpider.MovementStates
{
    public class MegaSpiderTangleAttackRState : MegaSpiderTangleAttackBaseState
    {
        public MegaSpiderTangleAttackRState(
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
    }
}
