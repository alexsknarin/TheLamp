using UnityEngine;

namespace _GAME.Scripts.Enemies.Megaspider.MovementStates
{
    public class MegaspiderSwingRState : MegaspiderSwingBaseState
    {
        public MegaspiderSwingRState(
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
