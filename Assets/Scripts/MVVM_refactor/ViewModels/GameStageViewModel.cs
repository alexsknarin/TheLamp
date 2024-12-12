using UnityEngine;

public class GameStageViewModel : IDisposable
{
    GameModel _gameModel;
    public Observable<GameStageState> CurrentGameStageState = new Observable<GameStageState>();
    
    public GameStageViewModel(GameModel gameModel)
    {
        _gameModel = gameModel;
        _gameModel.OnGameStageStateChangedEvent += OnGameStageStateChanged;
    }

    public void Dispose()
    {
        _gameModel.OnGameStageStateChangedEvent -= OnGameStageStateChanged;
    }

    private void OnGameStageStateChanged(GameStageState newState)
    {
        CurrentGameStageState.Value = newState;
    }

    public void HandleCurrentStageStateFinished()
    {
        _gameModel.HandleCurrentStageStateFinished();
    }
}
