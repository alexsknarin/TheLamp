using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Ladybug.MovementStates
{
    public class LadybugMovementPatrolStateL: FLadybugMovementPatrolState
    {
        public LadybugMovementPatrolStateL(
            Vector3 cameraPosition,
            IPositionDirectionProvider positionDirectionProvider,
            ILampPositionProviderService lampPositionProviderService,
            LadybugLampPositionsHolder lampPositionsHolder,
            float speed,
            float radius,
            float verticalAmplitude) : 
            base(
                cameraPosition, 
                positionDirectionProvider, 
                lampPositionProviderService, 
                lampPositionsHolder,
                speed, 
                radius, 
                verticalAmplitude)
        {
        }
    
        public override void OnEnter()
        {
            HandleEnter(-1);
        }
    
        public override void Tick()
        {
            HandleTick(-1);
        }
    }
}
