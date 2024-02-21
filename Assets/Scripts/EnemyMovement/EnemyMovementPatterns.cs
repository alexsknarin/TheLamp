using UnityEngine;

public class EnemyMovementPatterns
{
    public static Vector3 CircleMotion(float offsetAngle, float finalXRadius, float radius, float verticalAmplitude, float phase)
    {
        Vector3 newPosition = Vector3.zero;
        newPosition.x = Mathf.Cos(phase + offsetAngle) * finalXRadius;
        newPosition.y = Mathf.Sin(phase + offsetAngle) * radius * verticalAmplitude;

        return newPosition; 
    }
}
