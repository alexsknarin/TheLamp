using System;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megaspider.MovementStates
{
    public class MegaspiderDeathFallState: MegaspiderFallState
    {
        // TODO: custom Death movement behaviour
        public MegaspiderDeathFallState(
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
        
        public event Action Ended;

        public override void Exit()
        {
            base.Exit();
            Ended?.Invoke();
        }

        
    }
}
