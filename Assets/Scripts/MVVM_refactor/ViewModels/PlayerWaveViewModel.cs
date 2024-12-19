using System;

public class PlayerWaveViewModel : IDisposable
{
    private GameModel _gameModel;
    public PlayerWaveViewModel(GameModel gameModel)
    {
        _gameModel = gameModel;
        _gameModel.OnLampAttackStartedEvent += StartLampAttack;
    }

    public void Dispose()
    {
        _gameModel.OnLampAttackStartedEvent -= StartLampAttack;
    }

    public event Action<float> OnAttackStartEvent;

    private void StartLampAttack(float currentPower)
    {
        OnAttackStartEvent?.Invoke(currentPower);
    }
}
