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
    [SerializeField] private int _lampDamageWeightRight;
    [SerializeField] private int _lampDamageWeightLeft;
    [SerializeField] private int _lampDamageWeightBottom;
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
        _data.LampDamageWeightRight = _lampDamageWeightRight;
        _data.LampDamageWeightLeft = _lampDamageWeightLeft;
        _data.LampDamageWeightBottom = _lampDamageWeightBottom;
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
        _data.LampDamageWeightRight = currentData.LampDamageWeightRight;
        _data.LampDamageWeightLeft = currentData.LampDamageWeightLeft;
        _data.LampDamageWeightBottom = currentData.LampDamageWeightBottom;
        return _data;
    }
}
