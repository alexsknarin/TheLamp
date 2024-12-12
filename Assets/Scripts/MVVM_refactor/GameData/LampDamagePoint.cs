using System;

[Serializable]
public class LampDamagePoint
{
    public float Strength;
    public float LocalAngle;
    public float GlobalAngle;

    public LampDamagePoint()
    {
        Strength = 0;
        LocalAngle = 0;
        GlobalAngle = 0;
    }
}
