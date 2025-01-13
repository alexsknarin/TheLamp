using System;
using UnityEngine;

public class GameStageViewModel : IDisposable
{
    private GameModel _gameModel;
    private IGameConfigService _gameConfigService;
    public GameStageViewModel(GameModel gameModel, IGameConfigService gameConfigService)
    {
        _gameModel = gameModel;
        _gameConfigService = gameConfigService;
        _gameModel.GameStageStateChanged += OnGameStageStateChanged;
    }

    public void Dispose()
    {
        _gameModel.GameStageStateChanged -= OnGameStageStateChanged;
    }
    
    public delegate void IntroStartedEvent(bool skipStage, float duration, float normalizedHealth);
    public delegate void PrepareInStartedEvent(bool skipStage, float duration, bool isUpgradeUiRequired, int waveNum);
    public delegate void PrepareOutStartedEvent(bool skipStage, float duration);
    public delegate void GameOverInStartedEvent(bool skipStage, float duration, Vector3 lastEnemyPosition);
    public delegate void GameOverOutStartedEvent(bool skipStage, float duration);
    public event IntroStartedEvent IntroStarted;
    public event PrepareInStartedEvent PrepareInStarted;
    public event PrepareOutStartedEvent PrepareOutStarted;
    public event GameOverInStartedEvent GameOverInStarted;
    public event GameOverOutStartedEvent GameOverOutStarted;
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
    
    // Event Handle Methods
    private void OnGameStageStateChanged(GameStageState newState)
    {
        switch (newState)
        {
            case GameStageState.Intro:
                IntroStarted?.Invoke(
                    _gameConfigService.GameConfig.IntroStageSkip,
                    _gameConfigService.GameConfig.IntroStageDuration,
                    (float)_gameModel.LampHealth / (float)_gameModel.LampMaxHealth
                    );
                break;
            case GameStageState.PrepareIn:
                PrepareInStarted?.Invoke(
                    _gameConfigService.GameConfig.PrepareInStageSkip,
                    _gameConfigService.GameConfig.PrepareInStageDuration,
                    _gameModel.UpgradePoints>0, 
                    _gameModel.Wave
                    );
                break;
            case GameStageState.PrepareOut:
                PrepareOutStarted?.Invoke(
                    _gameConfigService.GameConfig.PrepareOutStageSkip,
                    _gameConfigService.GameConfig.PrepareOutStageDuration
                    );
                break;
            case GameStageState.GameOverIn:
                GameOverInStarted?.Invoke(
                    _gameConfigService.GameConfig.GameoverInStageSkip,
                    _gameConfigService.GameConfig.GameoverInStageDuration,
                    _gameModel.LastEnemyPosition
                    );
                break;
            case GameStageState.GameOverOut:
                GameOverOutStarted?.Invoke(
                    _gameConfigService.GameConfig.GameoverOutStageSkip,
                    _gameConfigService.GameConfig.GameoverOutStageDuration
                    );
                break;
            case GameStageState.Advertisement:
                AdvertisementStarted?.Invoke();
                break;
        }
    }
}
