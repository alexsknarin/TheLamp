using UnityEngine;
using UnityEngine.Serialization;

public class LampStatsManager : MonoBehaviour, IInitializable
{
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _currentHealth;
    [SerializeField] private float _initialCooldownTime;
    [SerializeField] private float _currentColldownTime;
    public int MaxHealth => _maxHealth;
    public int CurrentHealth => _currentHealth;
    public float NormalizedHealth => (float)_currentHealth / _maxHealth;
    public float InitialCooldownTime => _initialCooldownTime;
    public float CurrentColldownTime => _currentColldownTime;
    
    public void Initialize()
    {
        // Here we will read data from file
        _currentHealth = _maxHealth;
        _currentColldownTime = _initialCooldownTime;
    }
    
    public void UpdateCurrentHealth(int value)
    {
        _currentHealth += value;
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
