public class GameStateViewModel
{
    private GameModel _gameModel;
    private GameState _currentGameState;
    public GameState GameState => _currentGameState;
    
    public GameStateViewModel(GameModel gameModel)
    {
        _gameModel = gameModel;
        _currentGameState = _gameModel.CurrentGameState;
    }
}
