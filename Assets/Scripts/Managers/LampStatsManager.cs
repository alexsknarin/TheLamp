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
    public int MaxHealth => _maxHealth;
    public int CurrentHealth => _currentHealth;
    public float NormalizedHealth => (float)_currentHealth / _maxHealth;
    public float InitialCooldownTime => _initialCooldownTime;
    public float CurrentColldownTime => _currentColldownTime;
    public int UpgradePoints => _upgradePoints;
    
    public event Action OnHealthChange;
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
        // Here we will read data from file if there is one
        _currentHealth = _maxHealth;
        _currentColldownTime = _initialCooldownTime;
        _upgradePoints = 0;
        _upgradeThesholdIncrement = _upgradeThesholdInitialIncrement;
        _currentUpgradePointThreshold = _upgradeThesholdInitialIncrement;
    }
    
    private void UpdateUpgradePoints(int scores)
    {
        if (scores >= _currentUpgradePointThreshold)
        {
            _upgradePoints++;
            _currentUpgradePointThreshold += _upgradeThesholdIncrement;
            _upgradeThesholdIncrement++;
        }
    }
    
    public void UpgradeHealth()
    {
        if (_upgradePoints > 0)
        {
            _upgradePoints--;
            if (_currentHealth < _maxHealth)
            {
                _currentHealth++;
            }
            else if (_currentHealth == _maxHealth)
            {
                _maxHealth++;
                _currentHealth++;
            }
            OnHealthChange?.Invoke();
        }
    }
    
    public void UpgradeCooldown()
    {
        if (_upgradePoints > 0)
        {
            _upgradePoints--;
            _currentColldownTime -= _cooldownDecrement;
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
    }
}
