using UnityEngine;

namespace _GAME.Scripts.Enemies.Megaspider.MovementStates
{
    public class MegaspiderDropFallState : MegaspiderFallState
    {
        public MegaspiderDropFallState(
            Transform visibleBodyTransform,
            Transform calculatedTransform,
            Transform lampTransform,
            float exitYCoordinate,
            bool isFreeFall
            ) : base(
                visibleBodyTransform, 
                calculatedTransform,
                lampTransform,
                exitYCoordinate,
                isFreeFall
                )
        {
        }
    }
}
