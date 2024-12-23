[System.Serializable]
public class GameState
{
    public int Wave;
    public int LampHealth;
    public int LampMaxHealth;
    public float LampCooldownTime;
    public float LampAttackDistance;
    public int LampLevel;
    public int LampUpgradePoints;
    public UpgradeData UpgradeData;
    public GlassDamageData GlassDamageData; 
}
