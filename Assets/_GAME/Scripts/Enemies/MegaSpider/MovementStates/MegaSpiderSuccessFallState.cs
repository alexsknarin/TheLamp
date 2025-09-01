using UnityEngine;

namespace _GAME.Scripts.Enemies.Megaspider.MovementStates
{
    public class MegaspiderSuccessFallState : MegaspiderFallState
    {
        public MegaspiderSuccessFallState(
            Transform visibleBodyTransform,
            Transform calculatedTransform,
            Transform lampTransform,
            Transform rootTransform,
            float exitYCoordinate,
            bool isFreeFall
            ) : base(
                visibleBodyTransform, 
                calculatedTransform,
                lampTransform,
                rootTransform,
                exitYCoordinate,
                isFreeFall
                )
        {
        }
    }
}
