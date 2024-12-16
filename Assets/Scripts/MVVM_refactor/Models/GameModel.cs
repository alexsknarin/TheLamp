using System;
using UnityEngine;

public class GameModel
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
    
    
    public GameModel(GameState _gameState)
    {
        _currentGameState = _gameState;
        Debug.Log("GameModel created");
        Debug.Log("GameState: " + _gameState.LampCooldownTime);
    }
    
    public void Start()
    {
        Debug.Log("!!!!! The Game Has Been Started !!!!!");
        // Start the game
        CurrentGameStageState = GameStageState.IntroAnimation;
    }
    
    public void HandleCurrentStageStateFinished()
    {
        switch (_currentGameStageState)
        {
            case GameStageState.IntroAnimation:
                CurrentGameStageState = GameStageState.Wave;
                break;
            case GameStageState.Wave:
                CurrentGameStageState = GameStageState.PrepareInAnimation;
                break;
            case GameStageState.PrepareInAnimation:
                CurrentGameStageState = GameStageState.GameOverAnimation;
                break;
            case GameStageState.GameOverAnimation:
                Debug.Log("<<<<<<<   Game Finished.  >>>>>>>");
                break;
        }
    }
}
