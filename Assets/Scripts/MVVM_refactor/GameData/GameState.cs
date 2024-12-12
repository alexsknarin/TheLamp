using System;

[Serializable]
public class GameState
{
    public int Wave;
    public int Scores;
    public int LampHealth;
    public int LampMaxHealth;
    public float LampCooldownTime;
    public float LampAttackDistance;
    public int LampLevel;
    public int LampXp;
    public int LampXpThreshold;
    public int LampXpIncrement;
    public int ImpactLastPointNumber;
    public int LampDamageWeightRight;
    public int LampDamageWeightLeft;
    public int LampDamageWeightBottom;
    public LampDamagePoint LampDamagePoint01;
    public LampDamagePoint LampDamagePoint02;
    public LampDamagePoint LampDamagePoint03;
}
