using UnityEngine;

namespace _GAME.Scripts.Enemies.MegaSpider.MovementStates
{
    public class MegaSpiderSwingRState : MegaSpiderSwingBaseState
    {
        public MegaSpiderSwingRState(
            Transform visibleBodyTransform, 
            Transform calculatedTransform, 
            bool isLeftSide
            ) : base(
            visibleBodyTransform, 
            calculatedTransform,
            isLeftSide
            )
        {
        }
    }
}
