using UnityEngine;

public class TrajectoryNoise
{
    public static Vector3 Generate(float frequency)
    {
        Vector3 noiseValue = Vector3.zero;
        noiseValue.x = Mathf.PerlinNoise(frequency * Time.time, 0) * 2 - 1;
        noiseValue.y = Mathf.PerlinNoise(0, frequency * Time.time) * 2 - 1;
        return noiseValue;
    }
}
