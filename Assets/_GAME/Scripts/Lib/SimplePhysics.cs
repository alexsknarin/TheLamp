using UnityEngine;

namespace _GAME.Scripts.Lib
{
    public abstract class SimplePhysics
    {
        public static Vector3 Fall(
            Vector3 initialDirection,
            ref float outForceMagnitude,
            ref float gravityMagnitude,
            bool isFreeFall,
            float initialOutForceMagnitude,
            float outForceIncrement,
            float gravityIncrement
            )
        {
            float timeStep = Time.deltaTime;
            
            Vector3 result = Vector3.down * (gravityMagnitude * timeStep);
            
            // Accelerate fall
            gravityMagnitude += gravityIncrement * timeStep; 
            
            if (isFreeFall)
                return result;
            
            result += initialDirection * (outForceMagnitude * timeStep);
            
            // Decrement initial out force until it reaches 0
            outForceMagnitude -= outForceIncrement * timeStep;
            outForceMagnitude = Mathf.Clamp(outForceMagnitude, 0f, initialOutForceMagnitude);
            
            return result;
        }
    }
}
