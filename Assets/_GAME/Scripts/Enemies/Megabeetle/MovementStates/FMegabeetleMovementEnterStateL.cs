using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megabeetle.MovementStates
{
    public class FMegabeetleMovementEnterStateL : FMegabeetleMovementEnterState
    {
        public FMegabeetleMovementEnterStateL(
            Vector3 cameraPosition,
            IPositionDirectionProvider positionDirectionProvider,
            ILampPositionProviderService lampPositionProviderService, 
            float speed,
            float radius,
            float verticalAmplitude) : 
            base(
                cameraPosition,
                positionDirectionProvider,
                lampPositionProviderService,
                speed,
                radius,
                verticalAmplitude
            )
        {
        }

        public override void Enter()
        {
            HandleEnter(-1);
        }

        public override void Tick()
        {
            HandleTick(-1);
        }
    }
}