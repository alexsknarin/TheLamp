using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megamothling.MovementStates
{
    public class MegamothlingMovementReturnState : MegamothlingMovementEnterState
    {
        public MegamothlingMovementReturnState(
            Vector3 cameraPosition, 
            IPositionDirectionProvider positionDirectionProvider, 
            float speed, 
            float radius, 
            float verticalAmplitude, 
            bool isFirstEnter) : 
            base(cameraPosition, positionDirectionProvider, speed, radius, verticalAmplitude, isFirstEnter)
        {
        }
    }
}
