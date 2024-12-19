using System;

public class PlayerWaveViewModel : IDisposable
{
    public Observable<float> Power = new Observable<float>();
    
    private GameModel _gameModel;
    public PlayerWaveViewModel(GameModel gameModel)
    {
        _gameModel = gameModel;
        _gameModel.OnLampAttackStartedEvent += StartLampAttack;
        _gameModel.OnPowerChangedEvent += UpdatePower;
    }

    public void Dispose()
    {
        _gameModel.OnLampAttackStartedEvent -= StartLampAttack;
        _gameModel.OnPowerChangedEvent -= UpdatePower;
    }

    private void UpdatePower(float power)
    {
        Power.Value = power;
    }

    public event Action<float> OnAttackStartEvent;

    private void StartLampAttack(float currentPower)
    {
        OnAttackStartEvent?.Invoke(currentPower);
    }
}
