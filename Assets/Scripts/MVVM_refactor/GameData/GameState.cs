using System;

[Serializable]
public class GameState
{
    public int Wave;
    public int LampHealth;
    public int LampMaxHealth;
    public float LampCooldownTime;
    public float LampAttackDistance;
    public int Scores;
    public int LampLevel;
    public int LampXp;
    public int LampXpThreshold;
    public int LampXpIncrement;
    public GlassDamageData GlassDamageData; 
}
