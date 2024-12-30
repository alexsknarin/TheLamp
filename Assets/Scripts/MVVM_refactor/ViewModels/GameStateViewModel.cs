using System;
using UnityEngine;

public class GameStateViewModel: IDisposable
{
    private GameModel _gameModel;
    private GameState _currentGameState;
    public GameState CurrentGameState => _currentGameState;
    public event Action<GameState> OnGameStateChangedEvent; 
    
    public GameStateViewModel(GameModel gameModel)
    {
        _gameModel = gameModel;
        _currentGameState = _gameModel.CurrentGameState;
        _gameModel.OnGameStateChangedEvent += OnGameStateChanged;
    }

    public void Dispose()
    {
        _gameModel.OnGameStateChangedEvent -= OnGameStateChanged;
    }

    private void OnGameStateChanged(GameState state)
    {
        _currentGameState = state;
        OnGameStateChangedEvent?.Invoke(_currentGameState);
    }
}
