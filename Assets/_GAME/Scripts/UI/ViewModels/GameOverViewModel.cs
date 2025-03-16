using _GAME.Scripts.GameCoreSystems;

namespace _GAME.Scripts.UI.ViewModels
{
    public class GameOverViewModel
    {
        private GameModel _gameModel;
    
        public GameOverViewModel(GameModel gameModel)
        {
            _gameModel = gameModel;
        }
    
        public void RestartGameWitAd()
        {
            _gameModel.HandleRestartGameWitAdFromGameOver();
        }
    
        public void RestartGame()
        {
            _gameModel.HandleRestartGameNoAdFromGameOver();
        }
    
        public void ExitGame()
        {
            _gameModel.ExitGame();
        }
    }
}
