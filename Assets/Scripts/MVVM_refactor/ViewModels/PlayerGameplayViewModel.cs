using System;

public class PlayerGameplayViewModel : IDisposable
{
    public Observable<float> Power = new();
    public Observable<float> LampNormalizedHealth = new();
    public Observable<bool> IsBlocked = new();
    public Observable<float> AttackDistance = new();
    private int _currentHealth = 8;
    
    private GameModel _gameModel;

    public PlayerGameplayViewModel(GameModel gameModel)
    {
        _gameModel = gameModel;
        _gameModel.LampAttackStarted += OnLampAttackStarted;
        _gameModel.PowerChanged += OnPowerChanged;
        _gameModel.LampHealthChanged += OnLampHealthChanged;
        _gameModel.LampMaxHealthChanged += OnLampMaxHealthChanged;
        _gameModel.LampBlockedModeSet += OnLampBlockedModeSet;
        _gameModel.LampDamageStarted += OnLampDamageStarted;
        _gameModel.LampGlassDamageChanged += OnLampGlassDamageChanged;
        _gameModel.LampAttackDistanceChanged += OnLampAttackDistanceChanged;
    }

    public void Dispose()
    {
        _gameModel.LampAttackStarted -= OnLampAttackStarted;
        _gameModel.PowerChanged -= OnPowerChanged;
        _gameModel.LampHealthChanged -= OnLampHealthChanged;
        _gameModel.LampMaxHealthChanged -= OnLampMaxHealthChanged;
        _gameModel.LampBlockedModeSet -= OnLampBlockedModeSet;
        _gameModel.LampDamageStarted -= OnLampDamageStarted;
        _gameModel.LampGlassDamageChanged -= OnLampGlassDamageChanged;
        _gameModel.LampAttackDistanceChanged -= OnLampAttackDistanceChanged;
    }

    public Action<GlassDamageData> LampGlassDamageChanged;
    public event Action LastHealthPointStarted;
    public event Action LastHealthPointEnded;
    public event Action<float, bool> AttackStart;
    public event Action<float> LampDamaged;
    public event Action LampDied;
    public event Action HealthUpgraded;


    public void OnDamageStateEnded() // TODO: com up with name
    {
        _gameModel.HandleDamageStateEnded();
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

    private void OnLampBlockedModeSet(bool isBlocked)
    {
        IsBlocked.Value = isBlocked;
    }

    private void OnPowerChanged(float power)
    {
        Power.Value = power;
    }
    
    /// <summary>
    /// Start Lamp Attack.
    /// </summary>
    /// <param name="currentPower"></param>
    private void OnLampAttackStarted(float currentPower)
    {
        AttackStart?.Invoke(currentPower, _gameModel.IsLampBlocked);
    }

    private void OnLampHealthChanged(int newHealth)
    {
        
        _currentHealth = newHealth;
        LampNormalizedHealth.Value = (float)_currentHealth / _gameModel.LampMaxHealth;
        
        // check for the last health point
        if (newHealth == 1)
        {
            LastHealthPointStarted?.Invoke();
        }
        else
        {
            LastHealthPointEnded?.Invoke();
        }
    }

    private void OnLampMaxHealthChanged(int newMaxPoints)
    {   
        HealthUpgraded?.Invoke();
    }

    private void OnLampDamageStarted(float duration)
    {
        LampDamaged?.Invoke(duration);
    }

    private void OnLampGlassDamageChanged(GlassDamageData damageData)
    {
        LampGlassDamageChanged?.Invoke(damageData);
    }

    private void OnLampAttackDistanceChanged(float distance)
    {
        AttackDistance.Value = distance;
    }
}
