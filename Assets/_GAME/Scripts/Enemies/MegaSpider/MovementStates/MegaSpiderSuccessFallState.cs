using System;
using _GAME.Scripts.Lib.Interfaces;
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
            Transform cameraTransform,
            IGameConfigService configService,
            float exitYCoordinate,
            bool isFreeFall
            ) : base(
                visibleBodyTransform, 
                calculatedTransform,
                lampTransform,
                rootTransform,
                cameraTransform,
                configService,
                exitYCoordinate,
                isFreeFall
                )
        {
        }
    }
}
