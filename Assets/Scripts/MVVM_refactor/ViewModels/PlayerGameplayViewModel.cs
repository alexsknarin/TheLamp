using System;
using UnityEngine;

public class PlayerGameplayViewModel : IDisposable
{
    public Observable<float> Power = new Observable<float>();
    public Observable<float> LampNormalizedHealth = new Observable<float>();
    public Observable<bool> IsBlocked = new Observable<bool>();

    public Action<GlassDamageData> OnLampGlassDamageChangedEvent;
    
    public event Action OnLastHealthPointStartedEvent;
    public event Action OnLastHealthPointEndedEvent;
    public event Action<float, bool> OnAttackStartEvent;
    public event Action<float> OnLampDamagedEvent;
    public event Action OnLampDeadEvent;
    
    private GameModel _gameModel;
    
    private int _currentHealth = 8;
    private int _previousHealth = 8;
    
    public PlayerGameplayViewModel(GameModel gameModel)
    {
        _gameModel = gameModel;
        _gameModel.OnLampAttackStartedEvent += StartLampAttack;
        _gameModel.OnPowerChangedEvent += UpdatePower;
        _gameModel.OnLampHealthChangedEvent += UpdateLampHealth;
        _gameModel.OnLampBlockedModeSetEvent += SetBlockedMode;
        _gameModel.OnLampDamageStartedEvent += StartLampDamage;
        _gameModel.OnLampDeathEvent += StartLampDeath;
        _gameModel.OnLampGlassDamageChangedEvent += UpdateLampGlassDamage;
    }

    public void Dispose()
    {
        _gameModel.OnLampAttackStartedEvent -= StartLampAttack;
        _gameModel.OnPowerChangedEvent -= UpdatePower;
        _gameModel.OnLampHealthChangedEvent -= UpdateLampHealth;
        _gameModel.OnLampBlockedModeSetEvent -= SetBlockedMode;
        _gameModel.OnLampDamageStartedEvent -= StartLampDamage;
        _gameModel.OnLampDeathEvent -= StartLampDeath;
        _gameModel.OnLampGlassDamageChangedEvent -= UpdateLampGlassDamage;
    }

    public void HandleDamageStateEnded()
    {
        _gameModel.HandleDamageStateEnded();
    }

    private void SetBlockedMode(bool isBlocked)
    {
        IsBlocked.Value = isBlocked;
    }

    private void UpdatePower(float power)
    {
        Power.Value = power;
    }

    private void StartLampAttack(float currentPower)
    {
        OnAttackStartEvent?.Invoke(currentPower, _gameModel.IsLampBlocked);
    }

    private void UpdateLampHealth(int newHealth)
    {
        _previousHealth = _currentHealth;
        _currentHealth = newHealth;
        LampNormalizedHealth.Value = (float)_currentHealth / _gameModel.LampMaxHealth;
        
        if (newHealth == 1)
        {
            OnLastHealthPointStartedEvent?.Invoke();
        }
        else if (_previousHealth == 1 && newHealth > 1)
        {
            OnLastHealthPointEndedEvent?.Invoke();
        }
    }

    private void StartLampDamage(float duration)
    {
        OnLampDamagedEvent?.Invoke(duration);
    }

    private void StartLampDeath()
    {
        OnLampDeadEvent?.Invoke();
    }

    private void UpdateLampGlassDamage(GlassDamageData damageData)
    {
        OnLampGlassDamageChangedEvent?.Invoke(damageData);
    }
}
