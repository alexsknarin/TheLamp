using System;

public class PlayerGameplayViewModel : IDisposable
{
    public Observable<float> Power = new Observable<float>();
    public Observable<float> LampNormalizedHealth = new Observable<float>();
    public event Action OnLastHealthPointStartedEvent;
    public event Action OnLastHealthPointEndedEvent;
    
    private GameModel _gameModel;
    
    private int _currentHealth = 8;
    private int _previousHealth = 8;
    
    public PlayerGameplayViewModel(GameModel gameModel)
    {
        _gameModel = gameModel;
        _gameModel.OnLampAttackStartedEvent += StartLampAttack;
        _gameModel.OnPowerChangedEvent += UpdatePower;
        _gameModel.OnLampHealthChangedEvent += UpdateLampHealth;
    }

    public void Dispose()
    {
        _gameModel.OnLampAttackStartedEvent -= StartLampAttack;
        _gameModel.OnPowerChangedEvent -= UpdatePower;
        _gameModel.OnLampHealthChangedEvent -= UpdateLampHealth;
    }

    private void UpdatePower(float power)
    {
        Power.Value = power;
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

    public event Action<float> OnAttackStartEvent;

    private void StartLampAttack(float currentPower)
    {
        OnAttackStartEvent?.Invoke(currentPower);
    }
}
