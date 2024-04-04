using UnityEngine;

[CreateAssetMenu(fileName = "SaveDataContainer", menuName = "SaveDataContainer")]
public class SaveDataContainer : ScriptableObject
{
    public int Wave;
    public int CurrentScore;
    public int Level;
    public int UpgradePoints;
    public int UpgradePointsThreshold;
    public int MaxHealth;
    public int Health;
    public float CooldownTime;
    
}
