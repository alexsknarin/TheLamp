using System;
using UnityEngine;

public class GameStateViewModel: IDisposable
{
    private GameModel _gameModel;
    private GameState _currentGameState;
    public GameState CurrentGameState => _currentGameState;
    public event Action<GameState> GameStateChanged; 
    
    public GameStateViewModel(GameModel gameModel)
    {
        _gameModel = gameModel;
        _currentGameState = _gameModel.CurrentGameState;
        _gameModel.GameStateChanged += OnGameStateChanged;
    }

    public void Dispose()
    {
        _gameModel.GameStateChanged -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState state)
    {
        _currentGameState = state;
        GameStateChanged?.Invoke(_currentGameState);
    }
}
