using _GAME.Scripts.GameCoreSystems.DataManagement.DataTypes;
using _GAME.Scripts.UI.ViewModels;
using UnityEngine;

namespace _GAME.Scripts.UI.Views
{
    public class GameStateView : MonoBehaviour
    {
        [SerializeField] private GameState _gameState;
        private GameStateViewModel _gameStateViewModel;
    
        public void Bind(GameStateViewModel viewModel)
        {
            _gameStateViewModel = viewModel;
            _gameState = _gameStateViewModel.CurrentGameState;        
        
            _gameStateViewModel.GameStateChanged += OnGameStateChanged;
        }

        private void OnDestroy()
        {
            _gameStateViewModel.GameStateChanged -= OnGameStateChanged;
        }

        private void OnGameStateChanged(GameState gameState)
        {
            _gameState = gameState;
        }
    }
}
