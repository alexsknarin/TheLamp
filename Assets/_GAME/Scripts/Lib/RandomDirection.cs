using UnityEngine;

public abstract class RandomDirection
{
    public static int Generate()
    {
        return Random.Range(0, 2) * 2 - 1;
    }
}
