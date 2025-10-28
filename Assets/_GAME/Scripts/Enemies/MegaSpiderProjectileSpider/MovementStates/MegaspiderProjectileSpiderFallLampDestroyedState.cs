using _GAME.Scripts.Enemies.MegaspiderProjectileSpider.MovementStates;
using UnityEngine;

namespace _GAME.Scripts.Enemies.MegaSpiderProjectileSpider.MovementStates
{
    public class MegaspiderProjectileSpiderFallLampDestroyedState : MegaspiderProjectileSpiderFallState
    {
        public MegaspiderProjectileSpiderFallLampDestroyedState(
            Transform bodyTransform, 
            Transform lampTransform, 
            float exitYCoordinate, 
            bool isFreeFall
            ) : base(
            bodyTransform, 
            lampTransform, 
            exitYCoordinate, 
            isFreeFall)
        {
        }
    }
}
