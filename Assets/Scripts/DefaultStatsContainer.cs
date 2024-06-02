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
    
    [SerializeField] private int ImpactLastPointNumber;
    
    [SerializeField] private float ImpactPoint01Strength;
    [SerializeField] private float ImpactPoint01LocalAngle;
    [SerializeField] private float ImpactPoint01GlobalAngle;
    
    [SerializeField] private float ImpactPoint02Strength;
    [SerializeField] private float ImpactPoint02LocalAngle;
    [SerializeField] private float ImpactPoint02GlobalAngle;
    
    [SerializeField] private float ImpactPoint03Strength;
    [SerializeField] private float ImpactPoint03LocalAngle;
    [SerializeField] private float ImpactPoint03GlobalAngle;
    
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
        
        _data.ImpactLastPointNumber = ImpactLastPointNumber;
        _data.ImpactPoint01Strength = ImpactPoint01Strength;
        _data.ImpactPoint01LocalAngle = ImpactPoint01LocalAngle;
        _data.ImpactPoint01GlobalAngle = ImpactPoint01GlobalAngle;
        _data.ImpactPoint02Strength = ImpactPoint02Strength;
        _data.ImpactPoint02LocalAngle = ImpactPoint02LocalAngle;
        _data.ImpactPoint02GlobalAngle = ImpactPoint02GlobalAngle;
        _data.ImpactPoint03Strength = ImpactPoint03Strength;
        _data.ImpactPoint03LocalAngle = ImpactPoint03LocalAngle;
        _data.ImpactPoint03GlobalAngle = ImpactPoint03GlobalAngle;
        
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
        _data.LampDamageWeightRight = 0;
        _data.LampDamageWeightLeft = 0;
        _data.LampDamageWeightBottom = 0;
        _data.ImpactLastPointNumber = 0;
        _data.ImpactPoint01Strength = 0;
        _data.ImpactPoint01LocalAngle = 0;
        _data.ImpactPoint01GlobalAngle = 0;
        _data.ImpactPoint02Strength = 0;
        _data.ImpactPoint02LocalAngle = 0;
        _data.ImpactPoint02GlobalAngle = 0;
        _data.ImpactPoint03Strength = 0;
        _data.ImpactPoint03LocalAngle = 0;
        _data.ImpactPoint03GlobalAngle = 0;
        
        return _data;
    }
}
