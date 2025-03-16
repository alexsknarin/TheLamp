using _GAME.Scripts.Lib.Interfaces;
using _GAME.Scripts.UI.ViewModels;
using UnityEngine;
using UnityEngine.UI;

namespace _GAME.Scripts.UI.Views
{
    public class GameOverViewUI : MonoBehaviour, IInitializable
    {
        [SerializeField] private Button _restartWithAdButton;
        [SerializeField] private Button _restartNoAdButton;
        [SerializeField] private Button _exitButton;
        private GameOverViewModel _gameOverViewModel;
    
        public void Bind(GameOverViewModel gameOverViewModel)
        {
            _gameOverViewModel = gameOverViewModel;
        }
    
        public void Initialize()
        {
            _restartNoAdButton.onClick.AddListener(_gameOverViewModel.RestartGame);
            _restartWithAdButton.onClick.AddListener(_gameOverViewModel.RestartGameWitAd);
            _exitButton.onClick.AddListener(_gameOverViewModel.ExitGame);
        }
    
        private void OnDestroy()
        {
            _restartNoAdButton.onClick.RemoveListener(_gameOverViewModel.RestartGame);
            _restartWithAdButton.onClick.RemoveListener(_gameOverViewModel.RestartGameWitAd);
            _exitButton.onClick.RemoveListener(_gameOverViewModel.ExitGame);
        }
    }
}
