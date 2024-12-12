using System;
using UnityEngine;

public class GameModel
{
    private GameState _gameState;
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
        _gameState = _gameState;
        Debug.Log("GameModel created");
        Debug.Log("GameState: " + _gameState.LampCooldownTime);
    }
    
    public void Start()
    {
        Debug.Log("!!!!! The Game Has Been Started !!!!!");
        // Start the game
        CurrentGameStageState = GameStageState.Intro;
    }
    
    public void HandleCurrentStageStateFinished()
    {
        switch (_currentGameStageState)
        {
            case GameStageState.Intro:
                CurrentGameStageState = GameStageState.Wave;
                break;
            case GameStageState.Wave:
                CurrentGameStageState = GameStageState.Prepare;
                break;
            case GameStageState.Prepare:
                CurrentGameStageState = GameStageState.Gameover;
                break;
            case GameStageState.Gameover:
                Debug.Log("<<<<<<<   Game Finished.  >>>>>>>");
                break;
        }
    }
}
