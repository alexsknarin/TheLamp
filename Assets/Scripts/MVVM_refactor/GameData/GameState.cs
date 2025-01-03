[System.Serializable]
public class GameState
{
    public GameState() { }
    public GameState(GameState gameState)
    {
        Wave = gameState.Wave;
        LampHealth = gameState.LampHealth;
        LampMaxHealth = gameState.LampMaxHealth;
        LampCooldownTime = gameState.LampCooldownTime;
        LampAttackDistance = gameState.LampAttackDistance;
        LampLevel = gameState.LampLevel;
        LampUpgradePoints = gameState.LampUpgradePoints;
        UpgradeData = gameState.UpgradeData;
        GlassDamageData = gameState.GlassDamageData;
    }
    
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
