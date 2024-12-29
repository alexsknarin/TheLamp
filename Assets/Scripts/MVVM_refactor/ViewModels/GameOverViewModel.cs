public class GameOverViewModel
{
    private GameModel _gameModel;
    
    public GameOverViewModel(GameModel gameModel)
    {
        _gameModel = gameModel;
    }
    
    public void RestartGameWitAd()
    {
        _gameModel.RestartGameWitAd();
    }
    
    public void RestartGame()
    {
        _gameModel.RestartGameNoAd();
    }
    
    public void ExitGame()
    {
        _gameModel.ExitGame();
    }
}
