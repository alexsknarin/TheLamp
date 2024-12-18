using System;
using UnityEngine;

public class GameModel : IDisposable
{
    private GameState _currentGameState;
    public GameState CurrentGameState
    {
        get => _currentGameState;
        private set
        {
            _currentGameState = value;
            OnGameStateChangedEvent?.Invoke(value);
        }
    }
    public event Action<GameState> OnGameStateChangedEvent; // TODO: do we need this event?
    
    
    private GameStageState _currentGameStageState = GameStageState.Loading;
    public GameStageState CurrentGameStageState
    {
        get => _currentGameStageState;
        private set
        {
            var oldValue = _currentGameStageState;
            _currentGameStageState = value;
            if (!oldValue.Equals(value))
            {
                OnGameStageStateChangedEvent?.Invoke(value);
            }
        }
    }
    public event Action<GameStageState> OnGameStageStateChangedEvent; 
    
    private EnemyController _enemyController;
    
    public GameModel(GameState _gameState, EnemyController enemyController)
    {
        _currentGameState = _gameState;
        _enemyController = enemyController;
        _enemyController.OnWaveEndEvent += HandleWaveEnd;
        Debug.Log("GameModel created");
        Debug.Log("GameState: " + _gameState.LampCooldownTime);
    }

    public void StartGame()
    {
        Debug.Log("!!!!! The Game Has Been Started !!!!!");
        // Start the game
        CurrentGameStageState = GameStageState.Intro;
    }
    
    private void StartPrepareIn()
    {
        CurrentGameStageState = GameStageState.PrepareIn;
        Debug.Log("Starting PrepareIn");
    }
    
    private void StartPrepare()
    {
        CurrentGameStageState = GameStageState.Prepare;
        Debug.Log("Starting Prepare");
    }
    
    private void StartPrepareOut()
    {
        CurrentGameStageState = GameStageState.PrepareOut;
        Debug.Log("Starting PrepareOut");
    }
    
    private void StartWave()
    {
        CurrentGameStageState = GameStageState.Wave;
        _enemyController.StartWave();
    }

    public void HandleCurrentStageStateFinished() // TODO: viewModels Should not call this method directly
    {
        switch (_currentGameStageState)
        {
            case GameStageState.Intro:
                StartPrepareIn();
                break;
            case GameStageState.PrepareIn:
                StartPrepare();
                break;
            case GameStageState.Prepare:
                StartPrepareOut();
                break;
            case GameStageState.PrepareOut:
                StartWave();
                break;
            case GameStageState.Wave:
                CurrentGameStageState = GameStageState.GameOverOut;
                break;
            case GameStageState.GameOverOut:
                Debug.Log("<<<<<<<   Game Finished.  >>>>>>>");
                break;
        }
    }

    // TODO: rename public methods from Handle... to something else 
    public void HandleIntroEnd()
    {
        StartPrepareIn();    
    }
    
    public void HandlePrepareInEnd()
    {
        StartPrepare();
    }
    
    private void HandlePrepareEnd()
    {
        StartPrepareOut();
    }
    
    public void HandlePrepareOutEnd()
    {
        StartWave();
    }
    
    private void HandleWaveEnd()
    {
        HandleCurrentStageStateFinished();
    }

    public void HandleAttackButtonClicked()
    {
        if (_currentGameStageState == GameStageState.Prepare)
        {
            HandlePrepareEnd(); 
        }
        if (_currentGameStageState == GameStageState.Wave)
        {
            _enemyController.HandleAttackButtonClicked();
        }
        
    }

    public void Dispose()
    {
        throw new NotImplementedException();
    }
}
