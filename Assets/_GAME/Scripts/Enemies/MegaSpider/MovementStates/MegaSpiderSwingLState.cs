using UnityEngine;

namespace _GAME.Scripts.Enemies.MegaSpider.MovementStates
{
    public class MegaSpiderSwingLState : MegaSpiderSwingBaseState
    {
        public MegaSpiderSwingLState(
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
