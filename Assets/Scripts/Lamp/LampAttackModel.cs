using System;
using UnityEngine;

public class LampAttackModel : MonoBehaviour, IInitializable
{
    [SerializeField] private CircleCollider2D _attackZone;
    [SerializeField] private CircleCollider2D _attackExitZone;
    [SerializeField] private float _currentPower;
    [SerializeField] private float _maxPower;
    [SerializeField] private int _attackPower;
    [SerializeField] private float _attackDuration;
    [SerializeField] private float _fullCooldownTime;
    [SerializeField] private float _attackDistance;
    [SerializeField] private float _attackExitDistance;
    [SerializeField] private bool _isBlockedAttack = false;

    private LampStates _lampState = LampStates.Neutral;
    private float _localTime;
    
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
    
    public void Initialize()
    {
        _currentPower = _maxPower;
        _lampState = LampStates.Neutral;
        _attackZone.radius = _attackDistance;
        _attackExitZone.radius = _attackExitDistance;
        _isBlockedAttack = false;
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
        _isBlockedAttack = true;
    }
    
    public void RemoveAttackBlocker()
    {
        _isBlockedAttack = false;
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
            if (_isBlockedAttack)
            {
                OnLampBlockedAttack?.Invoke(_attackPower, _currentPower, _attackDuration, _attackDistance);
            }
            else
            {
                OnLampAttack?.Invoke(_attackPower, _currentPower, _attackDuration, _attackDistance);
            }
            _lampState = LampStates.Attack;
            _localTime = 0;
        }
    }

    private void PerformAttackState()
    {
        float phase = _localTime / _attackDuration;
        if (phase > 1) 
        {
            StartCooldownState();
        }
        _localTime += Time.deltaTime;
    }
    
    private void HandleDamageWithCooldown(EnemyBase enemy)
    {
        StartCooldownState();
        _localTime = 0;
    }
    
    private void StartCooldownState()
    {   
        _currentPower = 0;
        OnLampCurrentPowerChanged?.Invoke(_currentPower);
        _lampState = LampStates.Cooldown;
    }
    
    private void PerformCooldownState()
    {
        float phase = _localTime / _fullCooldownTime;
        if (phase > 1)
        {
            StartNeutralState();
            return;
        }
        _currentPower = phase;
        UpdateAttackPower();
        OnLampCurrentPowerChanged?.Invoke(_currentPower);
        _localTime += Time.deltaTime;
    }
    
    private void StartNeutralState()
    {
        _currentPower = _maxPower;
        OnLampCurrentPowerChanged?.Invoke(_currentPower);
        UpdateAttackPower();
        _lampState = LampStates.Neutral;
    }
}
