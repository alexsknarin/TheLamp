using System;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megaspider.MovementStates
{
    public class MegaspiderDeathFallState: MegaspiderFallState
    {
        public MegaspiderDeathFallState(
            Transform visibleBodyTransform,
            Transform calculatedTransform,
            Transform lampTransform,
            Transform rootTransform,
            Transform cameraTransform,
            float exitYCoordinate,
            bool isFreeFall
        ) : base(
            visibleBodyTransform, 
            calculatedTransform,
            lampTransform,
            rootTransform,
            cameraTransform,
            exitYCoordinate,
            isFreeFall
        )
        {
        }
        
        public event Action Ended;

        public override void Exit()
        {
            base.Exit();
            Ended?.Invoke();
        }

        
    }
}
