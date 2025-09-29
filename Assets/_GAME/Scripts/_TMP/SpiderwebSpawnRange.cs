using UnityEngine;

[System.Serializable]
public class SpiderwebSpawnRange
{
    public Vector3 p1;
    public Vector3 p2;

    public SpiderwebSpawnRange() { }
    
    public SpiderwebSpawnRange(float p1x, float p1y, float p1z, float p2x, float p2y, float p2z)
    {
        p1.x = p1x;
        p1.y = p1y;
        p1.z = p1z;
        p2.x = p2x;
        p2.y = p2y;
        p2.z = p2z;
    }
}
