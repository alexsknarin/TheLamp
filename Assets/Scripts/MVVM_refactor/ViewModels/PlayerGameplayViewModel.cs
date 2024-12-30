using System;
using UnityEngine;

public class PlayerGameplayViewModel : IDisposable
{
    public Observable<float> Power = new();
    public Observable<float> LampNormalizedHealth = new();
    public Observable<bool> IsBlocked = new();
    public Observable<float> AttackDistance = new();

    public Action<GlassDamageData> OnLampGlassDamageChangedEvent;
    
    public event Action OnLastHealthPointStartedEvent;
    public event Action OnLastHealthPointEndedEvent;
    public event Action<float, bool> OnAttackStartEvent;
    public event Action<float> OnLampDamagedEvent;
    public event Action OnLampDeadEvent;
    public event Action OnHealthUpgradedEvent;
    
    private GameModel _gameModel;
    
    private int _currentHealth = 8;
    
    public PlayerGameplayViewModel(GameModel gameModel)
    {
        _gameModel = gameModel;
        _gameModel.OnLampAttackStartedEvent += StartLampAttack;
        _gameModel.OnPowerChangedEvent += HandlePowerChange;
        _gameModel.OnLampHealthChangedEvent += UpdateLampHealth;
        _gameModel.OnLampMaxHealthChangedEvent += UpdateLampMaxHealth;
        _gameModel.OnLampBlockedModeSetEvent += SetBlockedMode;
        _gameModel.OnLampDamageStartedEvent += StartLampDamage;
        _gameModel.OnLampGlassDamageChangedEvent += UpdateLampGlassDamage;
        _gameModel.OnLampAttackDistanceChangedEvent += UpdateAttackDistance;
    }

    public void Dispose()
    {
        _gameModel.OnLampAttackStartedEvent -= StartLampAttack;
        _gameModel.OnPowerChangedEvent -= HandlePowerChange;
        _gameModel.OnLampHealthChangedEvent -= UpdateLampHealth;
        _gameModel.OnLampMaxHealthChangedEvent -= UpdateLampMaxHealth;
        _gameModel.OnLampBlockedModeSetEvent -= SetBlockedMode;
        _gameModel.OnLampDamageStartedEvent -= StartLampDamage;
        _gameModel.OnLampGlassDamageChangedEvent -= UpdateLampGlassDamage;
        _gameModel.OnLampAttackDistanceChangedEvent -= UpdateAttackDistance;
    }

    public void HandleDamageStateEnded()
    {
        _gameModel.HandleDamageStateEnded();
    }

    private void SetBlockedMode(bool isBlocked)
    {
        IsBlocked.Value = isBlocked;
    }

    private void HandlePowerChange(float power)
    {
        Power.Value = power;
    }

    private void StartLampAttack(float currentPower)
    {
        OnAttackStartEvent?.Invoke(currentPower, _gameModel.IsLampBlocked);
    }

    private void UpdateLampHealth(int newHealth)
    {
        
        _currentHealth = newHealth;
        LampNormalizedHealth.Value = (float)_currentHealth / _gameModel.LampMaxHealth;
        
        // check for the last health point
        if (newHealth == 1)
        {
            OnLastHealthPointStartedEvent?.Invoke();
        }
        else
        {
            OnLastHealthPointEndedEvent?.Invoke();
        }
    }

    private void UpdateLampMaxHealth(int newMaxPoints)
    {   
        OnHealthUpgradedEvent?.Invoke();
    }

    private void StartLampDamage(float duration)
    {
        OnLampDamagedEvent?.Invoke(duration);
    }

    private void UpdateLampGlassDamage(GlassDamageData damageData)
    {
        OnLampGlassDamageChangedEvent?.Invoke(damageData);
    }

    private void UpdateAttackDistance(float distance)
    {
        AttackDistance.Value = distance;
    }
    
    // Called from the view
    public void HandleExitButtonClicked()
    {
        _gameModel.ExitGame();
    }
    
    public void HandleRestartButtonClicked()
    {
        _gameModel.HandleImmediateRestartGame();
    }
}
