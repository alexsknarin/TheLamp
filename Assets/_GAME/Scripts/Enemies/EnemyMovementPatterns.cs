using UnityEngine;

namespace _GAME.Scripts.Enemies
{
    public class EnemyMovementPatterns
    {
        public static Vector2 CircleMotion(float offsetAngle, float finalXRadius, float radius, float verticalAmplitude, float phase)
        {
            Vector2 newPosition = Vector2.zero;
            newPosition.x = Mathf.Cos(phase + offsetAngle) * finalXRadius;
            newPosition.y = Mathf.Sin(phase + offsetAngle) * radius * verticalAmplitude;

            return newPosition; 
        }
    }
}
