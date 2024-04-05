 using System;
 using UnityEngine;

public class LampStatsManager : MonoBehaviour, IInitializable
{
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _currentHealth;
    [SerializeField] private float _initialCooldownTime;
    [SerializeField] private float _currentColldownTime;
    [SerializeField] private float _cooldownDecrement;
    [SerializeField] private int _upgradePoints;
    [SerializeField] private int _currentUpgradePointThreshold = 5;
    [SerializeField] private int _upgradeThesholdInitialIncrement = 5;
    [SerializeField] private int _upgradeThesholdIncrement;
    [SerializeField] private int _level;
    [SerializeField] private SaveDataContainer _saveDataContainer;
    public int MaxHealth => _maxHealth;
    public int CurrentHealth => _currentHealth;
    public float NormalizedHealth => (float)_currentHealth / _maxHealth;
    public float InitialCooldownTime => _initialCooldownTime;
    public float CurrentColldownTime => _currentColldownTime;
    public int UpgradePoints => _upgradePoints;
    
    public event Action OnHealthChange;
    public event Action OnHealthUpgraded;
    public event Action OnCooldownChange;

    private void OnEnable()
    {
        ScoresManager.OnScoreChange += UpdateUpgradePoints;
    }
    
    private void OnDisable()
    {
        ScoresManager.OnScoreChange -= UpdateUpgradePoints;
    }

    public void Initialize()
    {
        Debug.Log("LampStatsManager Initialize");
        _level = _saveDataContainer.Level;
        _currentHealth = _saveDataContainer.Health;
        _currentColldownTime = _saveDataContainer.CooldownTime;
        _upgradePoints = _saveDataContainer.UpgradePoints;
        _upgradeThesholdIncrement = _saveDataContainer.UpgradeThesholdIncrement;
        _currentUpgradePointThreshold = _saveDataContainer.UpgradePointsThreshold;
        
        
        // Here we will read data from file if there is one
        
        // _level = 1;
        // _currentHealth = _maxHealth;
        // _currentColldownTime = _initialCooldownTime;
        // _upgradePoints = 0;
        // _upgradeThesholdIncrement = _upgradeThesholdInitialIncrement;
        // _currentUpgradePointThreshold = _upgradeThesholdInitialIncrement;
        
        SaveData();
    }
    
    private void UpdateUpgradePoints(int scores)
    {
        if (scores >= _currentUpgradePointThreshold)
        {
            _upgradePoints++;
            _currentUpgradePointThreshold += _upgradeThesholdIncrement;
            _upgradeThesholdIncrement++;
            SaveData();
        }
    }
    
    public void UpgradeHealth()
    {
        if (_upgradePoints > 0)
        {
            _upgradePoints--;
            _level++;
            if (_currentHealth < _maxHealth)
            {
                _currentHealth++;
            }
            else if (_currentHealth == _maxHealth)
            {
                _maxHealth++;
                _currentHealth++;
                OnHealthUpgraded?.Invoke();
            }
            OnHealthChange?.Invoke();
            SaveData();
        }
    }
    
    public void UpgradeCooldown()
    {
        if (_upgradePoints > 0)
        {
            _level++;
            _upgradePoints--;
            _currentColldownTime -= _cooldownDecrement;
            SaveData();
            OnCooldownChange?.Invoke();
        }
    }
    
    public void DecreaseCurrentHealth(int value)
    {
        _currentHealth -= value;
        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
        }
        if (_currentHealth >= _maxHealth)
        {
            // Possibly raise MaxHealth - need testing
            _currentHealth = _maxHealth;
        }
        _saveDataContainer.Health = _currentHealth;
    }
    
    private void SaveData()
    {
        _saveDataContainer.Level = _level;
        _saveDataContainer.MaxHealth = _maxHealth;
        _saveDataContainer.Health = _currentHealth;
        _saveDataContainer.UpgradePoints = _upgradePoints;
        _saveDataContainer.UpgradePointsThreshold = _currentUpgradePointThreshold;
        _saveDataContainer.UpgradeThesholdIncrement = _upgradeThesholdIncrement;
        _saveDataContainer.CooldownTime = _currentColldownTime;
    }
}
