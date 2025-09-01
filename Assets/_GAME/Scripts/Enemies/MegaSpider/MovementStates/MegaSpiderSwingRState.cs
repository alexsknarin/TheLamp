using UnityEngine;

namespace _GAME.Scripts.Enemies.Megaspider.MovementStates
{
    public class MegaspiderSwingRState : MegaspiderSwingBaseState
    {
        public MegaspiderSwingRState(
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
