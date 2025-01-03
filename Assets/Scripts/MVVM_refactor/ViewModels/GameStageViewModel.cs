using System;
using UnityEngine;

public class GameStageViewModel : IDisposable
{
    private GameModel _gameModel;
    public GameStageViewModel(GameModel gameModel)
    {
        _gameModel = gameModel;
        _gameModel.GameStageStateChanged += OnGameStageStateChanged;
    }

    public void Dispose()
    {
        _gameModel.GameStageStateChanged -= OnGameStageStateChanged;
    }

    public event Action<float> IntroStarted;
    public event Action<bool, int> PrepareInStarted;
    public event Action PrepareOutStarted;
    public event Action<Vector3> GameOverInStarted;
    public event Action GameOverOutStarted;
    public event Action AdvertisementStarted;


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

    public void HandleGameOverInEnd()
    {
        _gameModel.HandleGameOverInEnd();    
    }

    public void HandleGameOverOutEnd()
    {
        _gameModel.HandleGameOverOutEnd();
    }

    public void HandleAdvertisementEnd()
    {
        _gameModel.HandleAdvertisementEnd();
    }
    

    private void OnGameStageStateChanged(GameStageState newState)
    {
        switch (newState)
        {
            case GameStageState.Intro:
                IntroStarted?.Invoke((float)_gameModel.LampHealth / (float)_gameModel.LampMaxHealth);
                break;
            case GameStageState.PrepareIn:
                PrepareInStarted?.Invoke(_gameModel.UpgradePoints>0, _gameModel.Wave);
                break;
            case GameStageState.PrepareOut:
                PrepareOutStarted?.Invoke();
                break;
            case GameStageState.GameOverIn:
                GameOverInStarted?.Invoke(_gameModel.LastEnemyPosition);
                break;
            case GameStageState.GameOverOut:
                GameOverOutStarted?.Invoke();
                break;
            case GameStageState.Advertisement:
                AdvertisementStarted?.Invoke();
                break;
        }
    }
}
