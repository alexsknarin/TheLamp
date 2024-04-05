using UnityEngine;

[CreateAssetMenu(fileName = "SaveDataContainer", menuName = "SaveDataContainer")]
public class SaveDataContainer : ScriptableObject
{
    public int Wave;
    public int CurrentScore;
    public int Level;
    public int UpgradePoints;
    public int UpgradePointsThreshold;
    public int UpgradeThesholdIncrement;
    public int MaxHealth;
    public int Health;
    public float CooldownTime;
    private SaveData _saveData;

    public void SetData(SaveData saveData)
    {
        Wave = saveData.Wave;
        CurrentScore = saveData.CurrentScore;
        Level = saveData.Level;
        UpgradePoints = saveData.UpgradePoints;
        UpgradePointsThreshold = saveData.UpgradePointsThreshold;
        UpgradeThesholdIncrement = saveData.UpgradeThesholdIncrement;
        MaxHealth = saveData.MaxHealth;
        Health = saveData.Health;
        CooldownTime = saveData.CooldownTime;
    }
    
    public SaveData GetData()
    {
        _saveData.Wave = Wave;
        _saveData.CurrentScore = CurrentScore;
        _saveData.Level = Level;
        _saveData.UpgradePoints = UpgradePoints;
        _saveData.UpgradePointsThreshold = UpgradePointsThreshold;
        _saveData.UpgradeThesholdIncrement = UpgradeThesholdIncrement;
        _saveData.MaxHealth = MaxHealth;
        _saveData.Health = Health;
        _saveData.CooldownTime = CooldownTime;
        return _saveData;
    }
}
