using UnityEngine;

namespace _GAME.Scripts.Enemies.Megaspider.MovementStates
{
    public class MegaspiderSwingLState : MegaspiderSwingBaseState
    {
        public MegaspiderSwingLState(
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
