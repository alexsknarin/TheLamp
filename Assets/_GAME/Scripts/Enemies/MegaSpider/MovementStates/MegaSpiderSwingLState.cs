using UnityEngine;

namespace _GAME.Scripts.Enemies.Megaspider.MovementStates
{
    public class MegaspiderSwingLState : MegaspiderSwingBaseState
    {
        public MegaspiderSwingLState(
            Transform visibleBodyTransform, 
            Transform calculatedTransform,
            Transform rootTransform,
            bool isLeftSide
            ) : base(
            visibleBodyTransform, 
            calculatedTransform,
            rootTransform,
            isLeftSide
            )
        {
        }
    }
}
