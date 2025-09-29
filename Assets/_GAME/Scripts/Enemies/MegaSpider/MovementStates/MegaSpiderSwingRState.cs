using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megaspider.MovementStates
{
    public class MegaspiderSwingRState : MegaspiderSwingBaseState
    {
        public MegaspiderSwingRState(
            Transform visibleBodyTransform, 
            Transform calculatedTransform,
            Transform rootTransform,
            IGameConfigService configService,
            bool isLeftSide
            ) : base(
            visibleBodyTransform, 
            calculatedTransform,
            rootTransform,
            configService,
            isLeftSide
            )
        {
        }
    }
}
