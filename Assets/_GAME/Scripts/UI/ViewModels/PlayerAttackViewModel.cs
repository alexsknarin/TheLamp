using _GAME.Scripts.GameCoreSystems;

namespace _GAME.Scripts.UI.ViewModels
{
    public class PlayerAttackViewModel
    {
        private GameModel _gameModel;
        public PlayerAttackViewModel(GameModel gameModel)
        {
            _gameModel = gameModel;
        }
        public void HandleAttackButtonClicked()
        {
            _gameModel.HandleAttackButtonClicked();
        }
    }
}
