using System;
using UnityEngine;

public class LampAttackModel : MonoBehaviour, IInitializable
{
    [SerializeField] private float _currentPower;
    [SerializeField] private float _maxPower;
    [SerializeField] private int _attackPower;
    [SerializeField] private float _attackDuration;
    [SerializeField] private float _attackDamageDurationFraction;     
    [SerializeField] private float _fullCooldownTime;
    [SerializeField] private float _attackDistance;
    [SerializeField] private int _attackBlockers = 0;
    
    private LampStates _lampState = LampStates.Neutral;
    
    private float _prevTime;
    
    public static event Action<int, float, float, float> OnLampAttack;
    public static event Action<int, float, float, float> OnLampBlockedAttack;
    public static event Action<float> OnLampCurrentPowerChanged;


    private void OnEnable()
    {
        PlayerInputHandler.OnPlayerAttack += StartAttackState;
        Lamp.OnLampDamaged += HandleDamageWithCooldown;
    }
    
    private void OnDisable()
    {
        PlayerInputHandler.OnPlayerAttack -= StartAttackState;
        Lamp.OnLampDamaged -= HandleDamageWithCooldown;
    }
    
    private void Update()
    {
        switch (_lampState)
        {
            case LampStates.Neutral:
                break;
            case LampStates.Attack:
                PerformAttackState();
                break;
            case LampStates.Cooldown:
                PerformCooldownState();
                break;
        }
    }
    
    public void AddAttackBlocker()
    {
        _attackBlockers++;
    }
    
    public void RemoveAttackBlocker()
    {
        _attackBlockers--;
    }
    
    public void Initialize()
    {
        _currentPower = _maxPower;
        _lampState = LampStates.Neutral;
    }
   
    private void UpdateAttackPower()
    {
        if (_currentPower < 0.3f)
        {
            _attackPower = 0;
        }
        else if(_currentPower < 0.6f)
        {
            _attackPower = 1;
        }
        else if (_currentPower < 0.9f)
        {
            _attackPower = 2;
        }
        else
        {
            _attackPower = 3;
        }
    }

    private void StartAttackState()
    {
        if (_lampState != LampStates.Attack)
        {
            if (_attackBlockers <= 0)
            {
                OnLampAttack?.Invoke(_attackPower, _currentPower, _attackDuration, _attackDistance);
            }
            else
            {
                OnLampBlockedAttack?.Invoke(_attackPower, _currentPower, _attackDuration, _attackDistance);
            }
            _lampState = LampStates.Attack;
            _prevTime = Time.time;
        }
    }

    private void PerformAttackState()
    {
        float phase = (Time.time - _prevTime) / _attackDuration;
        if (phase > 1) 
        {
            StartCooldownState();
        }
    }
    
    private void HandleDamageWithCooldown()
    {
        StartCooldownState();
        _prevTime = Time.time;
    }
    
    private void StartCooldownState()
    {   
        _currentPower = 0;
        OnLampCurrentPowerChanged?.Invoke(_currentPower);
        _lampState = LampStates.Cooldown;
    }
    
    private void PerformCooldownState()
    {
        float phase = (Time.time - _prevTime) / _fullCooldownTime;
        if (phase > 1)
        {
            StartNeutralState();
            return;
        }
        _currentPower = phase;
        UpdateAttackPower();
        OnLampCurrentPowerChanged?.Invoke(_currentPower);
    }
    
    private void StartNeutralState()
    {
        _currentPower = _maxPower;
        OnLampCurrentPowerChanged?.Invoke(_currentPower);
        UpdateAttackPower();
        _lampState = LampStates.Neutral;
    }


}
