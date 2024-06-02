     using System;
 using UnityEngine;
 using Random = UnityEngine.Random;

 public class LampStatsManager : MonoBehaviour, IInitializable
{
    private int _maxHealth;
    [Header("Stats displayed for debug purposes")]
    [Header("Use Default Data asset to change values")]
    [SerializeField] private int _currentHealth;
    private float _initialCooldownTime;
    [SerializeField] private float _currentColldownTime;
    [SerializeField] private float _cooldownDecrement;
    [SerializeField] private int _upgradePoints;
    [SerializeField] private int _currentUpgradePointThreshold = 5;
    [SerializeField] private int _upgradeThesholdInitialIncrement = 5;
    [SerializeField] private int _upgradeThesholdIncrement;
    [SerializeField] private int _level;
    [Header("Damage")]
    [SerializeField] private int _lampDamageWeightRight;
    [SerializeField] private int _lampDamageWeightLeft;
    [SerializeField] private int _lampDamageWeightBottom;
    [SerializeField] private Vector3 _damageWeights;
    [Header("Impact")]
    [SerializeField] private LampImpactPointsData _lampImpactPointsData = new LampImpactPointsData();
    public LampImpactPointsData LampImpactPointsData => _lampImpactPointsData;
    [Header("Save Data")]
    [SerializeField] private SaveDataContainer _saveDataContainer;
    
    public int MaxHealth => _maxHealth;
    public int CurrentHealth => _currentHealth;
    public float NormalizedHealth => (float)_currentHealth / _maxHealth;
    public float InitialCooldownTime => _initialCooldownTime;
    public float CurrentColldownTime => _currentColldownTime;
    public int UpgradePoints => _upgradePoints;
    public Vector3 DamageWeights => _damageWeights;
    
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
         // Add Read from SaveData
        _level = _saveDataContainer.Level;
        _currentHealth = _saveDataContainer.Health;
        _maxHealth = _saveDataContainer.MaxHealth;
        _currentColldownTime = _saveDataContainer.CooldownTime;
        _upgradePoints = _saveDataContainer.UpgradePoints;
        _upgradeThesholdIncrement = _saveDataContainer.UpgradeThesholdIncrement;
        _currentUpgradePointThreshold = _saveDataContainer.UpgradePointsThreshold;
        _lampDamageWeightRight = _saveDataContainer.LampDamageWeightRight;
        _lampDamageWeightLeft = _saveDataContainer.LampDamageWeightLeft;
        _lampDamageWeightBottom = _saveDataContainer.LampDamageWeightBottom;
        CalculateDamageWeghts();
        _lampImpactPointsData.ImpactLastPointNumber = _saveDataContainer.ImpactLastPointNumber;
        _lampImpactPointsData.ImpactPoint01Strength = _saveDataContainer.ImpactPoint01Strength;
        _lampImpactPointsData.ImpactPoint01LocalAngle = _saveDataContainer.ImpactPoint01LocalAngle;
        _lampImpactPointsData.ImpactPoint01GlobalAngle = _saveDataContainer.ImpactPoint01GlobalAngle;
        _lampImpactPointsData.ImpactPoint02Strength = _saveDataContainer.ImpactPoint02Strength;
        _lampImpactPointsData.ImpactPoint02LocalAngle = _saveDataContainer.ImpactPoint02LocalAngle;
        _lampImpactPointsData.ImpactPoint02GlobalAngle = _saveDataContainer.ImpactPoint02GlobalAngle;
        _lampImpactPointsData.ImpactPoint03Strength = _saveDataContainer.ImpactPoint03Strength;
        _lampImpactPointsData.ImpactPoint03LocalAngle = _saveDataContainer.ImpactPoint03LocalAngle;
        _lampImpactPointsData.ImpactPoint03GlobalAngle = _saveDataContainer.ImpactPoint03GlobalAngle;
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
                ReduceDamageWeights();
            }
            else if (_currentHealth == _maxHealth)
            {
                _maxHealth++;
                _currentHealth++;
                OnHealthUpgraded?.Invoke();
            }
            _lampImpactPointsData.Reset();
            SaveData();
            OnHealthChange?.Invoke();
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
    
    public void DecreaseCurrentHealth(int value, Vector3 attackDirection)
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
        
        // Lamp Damage stats update
        if (attackDirection.x >= 0f && attackDirection.y >= -0.5f)
        {
            _lampDamageWeightRight++;
            _saveDataContainer.LampDamageWeightRight = _lampDamageWeightRight;
        }
        else if (attackDirection.x < 0f && attackDirection.y >= -0.5f)
        {
            _lampDamageWeightLeft++;
            _saveDataContainer.LampDamageWeightLeft = _lampDamageWeightLeft;
        }
        else if (attackDirection.y < -0.5f)
        {
            _lampDamageWeightBottom++;
            _saveDataContainer.LampDamageWeightBottom = _lampDamageWeightBottom;
        }
        CalculateDamageWeghts();
        
        // Calculate Impact points
        _lampImpactPointsData.ImpactLastPointNumber++;
        if (_lampImpactPointsData.ImpactLastPointNumber > 6)
        {
            _lampImpactPointsData.ImpactLastPointNumber = 1;
        }
        _saveDataContainer.ImpactLastPointNumber = _lampImpactPointsData.ImpactLastPointNumber;
        
        float impactRandomLocalAngle = Random.Range(0, 6.2832f);
        float impactGlobalAngle = Mathf.Acos(attackDirection.x);
        if (attackDirection.y < 0)
        {
            impactGlobalAngle = Mathf.PI * 2 - impactGlobalAngle;
        }

        switch (_lampImpactPointsData.ImpactLastPointNumber)
        {
            case 1:
                _lampImpactPointsData.ImpactPoint01Strength = 1f;
                _lampImpactPointsData.ImpactPoint01LocalAngle = impactRandomLocalAngle;
                _lampImpactPointsData.ImpactPoint01GlobalAngle = impactGlobalAngle;
                // Save data
                _saveDataContainer.ImpactPoint01Strength = _lampImpactPointsData.ImpactPoint01Strength;
                _saveDataContainer.ImpactPoint01LocalAngle = _lampImpactPointsData.ImpactPoint01LocalAngle;
                _saveDataContainer.ImpactPoint01GlobalAngle = _lampImpactPointsData.ImpactPoint01GlobalAngle;
                break;
            case 2:
                _lampImpactPointsData.ImpactPoint02Strength = 1f;
                _lampImpactPointsData.ImpactPoint01Strength = 0.66f;
                _lampImpactPointsData.ImpactPoint02LocalAngle = impactRandomLocalAngle;
                _lampImpactPointsData.ImpactPoint02GlobalAngle = impactGlobalAngle;
                // Save data
                _saveDataContainer.ImpactPoint01Strength = _lampImpactPointsData.ImpactPoint01Strength;
                _saveDataContainer.ImpactPoint02Strength = _lampImpactPointsData.ImpactPoint02Strength;
                _saveDataContainer.ImpactPoint02LocalAngle = _lampImpactPointsData.ImpactPoint02LocalAngle;
                _saveDataContainer.ImpactPoint02GlobalAngle = _lampImpactPointsData.ImpactPoint02GlobalAngle;
                break;
            case 3:
                _lampImpactPointsData.ImpactPoint03Strength = 1f;
                _lampImpactPointsData.ImpactPoint02Strength = 0.66f;
                _lampImpactPointsData.ImpactPoint01Strength = 0.33f;
                _lampImpactPointsData.ImpactPoint03LocalAngle = impactRandomLocalAngle;
                _lampImpactPointsData.ImpactPoint03GlobalAngle = impactGlobalAngle;
                // Save data
                _saveDataContainer.ImpactPoint01Strength = _lampImpactPointsData.ImpactPoint01Strength;
                _saveDataContainer.ImpactPoint02Strength = _lampImpactPointsData.ImpactPoint02Strength;
                _saveDataContainer.ImpactPoint03Strength = _lampImpactPointsData.ImpactPoint03Strength;
                _saveDataContainer.ImpactPoint03LocalAngle = _lampImpactPointsData.ImpactPoint03LocalAngle;
                _saveDataContainer.ImpactPoint03GlobalAngle = _lampImpactPointsData.ImpactPoint03GlobalAngle;
                break;
            case 4: 
                _lampImpactPointsData.ImpactPoint03Strength = 0.66f;
                _lampImpactPointsData.ImpactPoint02Strength = 0.33f;
                _lampImpactPointsData.ImpactPoint01Strength = 1f;
                _lampImpactPointsData.ImpactPoint01LocalAngle = impactRandomLocalAngle;
                _lampImpactPointsData.ImpactPoint01GlobalAngle = impactGlobalAngle;
                // Save data
                _saveDataContainer.ImpactPoint01Strength = _lampImpactPointsData.ImpactPoint01Strength;
                _saveDataContainer.ImpactPoint02Strength = _lampImpactPointsData.ImpactPoint02Strength;
                _saveDataContainer.ImpactPoint03Strength = _lampImpactPointsData.ImpactPoint03Strength;
                _saveDataContainer.ImpactPoint01LocalAngle = _lampImpactPointsData.ImpactPoint01LocalAngle;
                _saveDataContainer.ImpactPoint01GlobalAngle = _lampImpactPointsData.ImpactPoint01GlobalAngle;
                break;
            case 5:
                _lampImpactPointsData.ImpactPoint03Strength = 0.33f;
                _lampImpactPointsData.ImpactPoint02Strength = 1f;
                _lampImpactPointsData.ImpactPoint01Strength = 0.66f;
                _lampImpactPointsData.ImpactPoint02LocalAngle = impactRandomLocalAngle;
                _lampImpactPointsData.ImpactPoint02GlobalAngle = impactGlobalAngle;
                // Save data
                _saveDataContainer.ImpactPoint01Strength = _lampImpactPointsData.ImpactPoint01Strength;
                _saveDataContainer.ImpactPoint02Strength = _lampImpactPointsData.ImpactPoint02Strength;
                _saveDataContainer.ImpactPoint03Strength = _lampImpactPointsData.ImpactPoint03Strength;
                _saveDataContainer.ImpactPoint02LocalAngle = _lampImpactPointsData.ImpactPoint02LocalAngle;
                _saveDataContainer.ImpactPoint02GlobalAngle = _lampImpactPointsData.ImpactPoint02GlobalAngle;
                break;
            case 6:
                _lampImpactPointsData.ImpactPoint03Strength = 1f;
                _lampImpactPointsData.ImpactPoint02Strength = 0.66f;
                _lampImpactPointsData.ImpactPoint01Strength = 0.33f;
                _lampImpactPointsData.ImpactPoint03LocalAngle = impactRandomLocalAngle;
                _lampImpactPointsData.ImpactPoint03GlobalAngle = impactGlobalAngle;
                // Save data
                _saveDataContainer.ImpactPoint01Strength = _lampImpactPointsData.ImpactPoint01Strength;
                _saveDataContainer.ImpactPoint02Strength = _lampImpactPointsData.ImpactPoint02Strength;
                _saveDataContainer.ImpactPoint03Strength = _lampImpactPointsData.ImpactPoint03Strength;
                _saveDataContainer.ImpactPoint03LocalAngle = _lampImpactPointsData.ImpactPoint03LocalAngle;
                _saveDataContainer.ImpactPoint03GlobalAngle = _lampImpactPointsData.ImpactPoint03GlobalAngle;
                break;
        }
    }
    
    private void CalculateDamageWeghts()
    {
        _damageWeights.x = (float)_lampDamageWeightRight/_maxHealth;
        _damageWeights.y = (float)_lampDamageWeightLeft/_maxHealth;
        _damageWeights.z = (float)_lampDamageWeightBottom/_maxHealth;
    }
    
    private void ReduceDamageWeights()
    {
        if(_lampDamageWeightRight == 0 && _lampDamageWeightLeft == 0 && _lampDamageWeightBottom == 0)
        {
            return;
        }
        
        if (_lampDamageWeightRight > _lampDamageWeightLeft && _lampDamageWeightRight > _lampDamageWeightBottom)
        {
            _lampDamageWeightRight--;
            CalculateDamageWeghts();
            return;
        }
        else if (_lampDamageWeightLeft > _lampDamageWeightRight && _lampDamageWeightLeft > _lampDamageWeightBottom)
        {
            _lampDamageWeightLeft--;
            CalculateDamageWeghts();
            return;
        }
        else if (_lampDamageWeightBottom > _lampDamageWeightRight && _lampDamageWeightBottom > _lampDamageWeightLeft)
        {
            _lampDamageWeightBottom--;
            CalculateDamageWeghts();
            return;
        }

        if (_lampDamageWeightRight == _lampDamageWeightLeft)
        {
            _lampDamageWeightRight--;
            CalculateDamageWeghts();
            return;
        }
        else if (_lampDamageWeightRight == _lampDamageWeightBottom)
        {
            _lampDamageWeightBottom--;
            CalculateDamageWeghts();
            return;
        }
        else if (_lampDamageWeightLeft == _lampDamageWeightBottom)
        {
            _lampDamageWeightLeft--;
            CalculateDamageWeghts();
            return;
        }
        
        CalculateDamageWeghts();
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
        _saveDataContainer.LampDamageWeightRight = _lampDamageWeightRight;
        _saveDataContainer.LampDamageWeightLeft = _lampDamageWeightLeft;
        _saveDataContainer.LampDamageWeightBottom = _lampDamageWeightBottom;
        
        _saveDataContainer.ImpactLastPointNumber = _lampImpactPointsData.ImpactLastPointNumber;
        _saveDataContainer.ImpactPoint01Strength = _lampImpactPointsData.ImpactPoint01Strength;
        _saveDataContainer.ImpactPoint01LocalAngle = _lampImpactPointsData.ImpactPoint01LocalAngle;
        _saveDataContainer.ImpactPoint01GlobalAngle = _lampImpactPointsData.ImpactPoint01GlobalAngle;
        _saveDataContainer.ImpactPoint02Strength = _lampImpactPointsData.ImpactPoint02Strength;
        _saveDataContainer.ImpactPoint02LocalAngle = _lampImpactPointsData.ImpactPoint02LocalAngle;
        _saveDataContainer.ImpactPoint02GlobalAngle = _lampImpactPointsData.ImpactPoint02GlobalAngle;
        _saveDataContainer.ImpactPoint03Strength = _lampImpactPointsData.ImpactPoint03Strength;
        _saveDataContainer.ImpactPoint03LocalAngle = _lampImpactPointsData.ImpactPoint03LocalAngle;
        _saveDataContainer.ImpactPoint03GlobalAngle = _lampImpactPointsData.ImpactPoint03GlobalAngle;

    }
}
