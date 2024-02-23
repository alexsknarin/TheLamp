using System;
using UnityEngine;

public class LampAttackModel : MonoBehaviour
{
    [SerializeField] private float _currentPower;
    [SerializeField] private float _maxPower;
    [SerializeField] private int _attackPower;
    [SerializeField] private float _attackDuration;
    [SerializeField] private float _fullCooldownTime;
    
    private LampStates _lampState = LampStates.Neutral;
    
    private float _prevTime;
    
    public static event Action<int, float, float> OnLampAttack;
    public static event Action<float> OnLampCurrentPowerChanged;


    private void OnEnable()
    {
        PlayerInputHandler.OnPlayerAttack += StartAttackState;
    }
    
    private void OnDisable()
    {
        PlayerInputHandler.OnPlayerAttack -= StartAttackState;
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

    public void Init()
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
            OnLampAttack?.Invoke(_attackPower, _currentPower, _attackDuration);
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
        // TODO: Make Attack work over time
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
