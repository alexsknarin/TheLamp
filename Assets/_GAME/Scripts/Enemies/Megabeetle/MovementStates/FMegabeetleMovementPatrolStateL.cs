using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megabeetle.MovementStates
{
    public class FMegabeetleMovementPatrolStateL : FMegabeetleMovementPatrolState
    {
        public FMegabeetleMovementPatrolStateL(
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