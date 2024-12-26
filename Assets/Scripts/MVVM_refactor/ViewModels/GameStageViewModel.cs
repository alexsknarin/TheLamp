using System;
using UnityEngine;

public class GameStageViewModel : IDisposable
{
    private GameModel _gameModel;
    public event Action<float> OnIntroStartedEvent;
    public event Action<bool, int> OnPrepareInStartedEvent;
    public event Action OnPrepareOutStartedEvent;

    public GameStageViewModel(GameModel gameModel)
    {
        _gameModel = gameModel;
        _gameModel.OnGameStageStateChangedEvent += OnGameStageStateChanged;
    }

    public void Dispose()
    {
        _gameModel.OnGameStageStateChangedEvent -= OnGameStageStateChanged;
    }

    private void OnGameStageStateChanged(GameStageState newState)
    {
        switch (newState)
        {
            case GameStageState.Intro:
                OnIntroStartedEvent?.Invoke((float)_gameModel.LampHealth / (float)_gameModel.LampMaxHealth);
                break;
            case GameStageState.PrepareIn:
                OnPrepareInStartedEvent?.Invoke(_gameModel.UpgradePoints>0, _gameModel.Wave);
                break;
            case GameStageState.PrepareOut:
                OnPrepareOutStartedEvent?.Invoke();
                break;
        }
    }
    
    
    // External methods to call from views
    public void HandleIntroEnd()
    {
        _gameModel.HandleIntroEnd();    
    }
    
    public void HandlePrepareInEnd()
    {
        _gameModel.HandlePrepareInEnd();    
    }
    
    public void HandlePrepareOutEnd()
    {
        _gameModel.HandlePrepareOutEnd();    
    }
}
