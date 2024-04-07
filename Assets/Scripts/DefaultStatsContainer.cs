using UnityEngine;

[CreateAssetMenu(fileName = "DefaultStatsContainer", menuName = "DefaultStatsContainer")]
public class DefaultStatsContainer : ScriptableObject
{
    [SerializeField] private int _wave = 0;
    [SerializeField] private int _currentScore = 0;
    [SerializeField] private int _level = 0;
    [SerializeField] private int _upgradePoints = 0;
    [SerializeField] private int _upgradePointsThreshold = 5;
    [SerializeField] private int _upgradeThesholdIncrement = 5;
    [SerializeField] private int _maxHealth = 8;
    [SerializeField] private int _health = 8;
    [SerializeField] private float _cooldownTime = 3f;
    private SaveData _data = new SaveData();
    
    public SaveData GetData()
    {
        _data.Wave = _wave;
        _data.CurrentScore = _currentScore;
        _data.Level = _level;
        _data.UpgradePoints = _upgradePoints;
        _data.UpgradePointsThreshold = _upgradePointsThreshold;
        _data.UpgradeThesholdIncrement = _upgradeThesholdIncrement;
        _data.MaxHealth = _maxHealth;
        _data.Health = _health;
        _data.CooldownTime = _cooldownTime;
        return _data;
    }

    public SaveData GetDataKeepUpgrades(SaveData currentData)
    {
        _data.Wave = _wave;
        _data.CurrentScore = _currentScore;
        _data.Level = _level;
        _data.UpgradePoints = _upgradePoints;
        _data.UpgradePointsThreshold = _upgradePointsThreshold;
        _data.UpgradeThesholdIncrement = _upgradeThesholdIncrement;
        _data.MaxHealth = currentData.MaxHealth;
        _data.Health = currentData.MaxHealth;
        _data.CooldownTime = currentData.CooldownTime;
        return _data;
    }
}
