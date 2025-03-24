using System;
using _GAME.Scripts.GameCoreSystems;
using _GAME.Scripts.GameCoreSystems.DataManagement.DataTypes;
using IDisposable = _GAME.Scripts.Lib.Interfaces.IDisposable;

namespace _GAME.Scripts.UI.ViewModels
{
    public class GameStateViewModel: IDisposable
    {
        private GameModel _gameModel;
        private GameState _currentGameState;

        public GameStateViewModel(GameModel gameModel)
        {
            _gameModel = gameModel;
            _currentGameState = _gameModel.CurrentGameState;
            _gameModel.GameStateChanged += OnGameStateChanged;
        }
        public event Action<GameState> GameStateChanged;
        public GameState CurrentGameState => _currentGameState;

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
}
