using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megabeetle.MovementStates
{
    public class FMegabeetleMovementPatrolStateR : FMegabeetleMovementPatrolState
    {
        public FMegabeetleMovementPatrolStateR(
            Vector3 cameraPosition, 
            IPositionDirectionProvider positionDirectionProvider, 
            float speed, 
            float radius, 
            float verticalAmplitude) :
            base(
                cameraPosition, 
                positionDirectionProvider, 
                speed, 
                radius, 
                verticalAmplitude)
        {
        }

        public override void OnEnter()
        {
            HandleEnter(1);
        }

        public override void Tick()
        {
            HandleTick(1);
        }
    }
}
