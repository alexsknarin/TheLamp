using UnityEngine;

public class AdvertisementEventListener: IDisposable
{
    private IAdvertisementService _advertisementService;
    private GameModel _gameModel;
    
    public AdvertisementEventListener(
        IAdvertisementService advertisementService,
        GameModel gameModel
        )
    {
        _advertisementService = advertisementService;
        _gameModel = gameModel;
        
        _gameModel.GameStarted += OnGameStarted;
        _gameModel.GameStageStateChanged += OnGameStageStateChanged;
        _advertisementService.AdSuccessfullyFinished += _gameModel.HandleAdvertisementEnd;
    }

    public void Dispose()
    {
        _gameModel.GameStarted -= OnGameStarted;
        _gameModel.GameStageStateChanged -= OnGameStageStateChanged;
        _advertisementService.AdSuccessfullyFinished -= _gameModel.HandleAdvertisementEnd;
    }

    private void OnGameStarted()
    {
        _advertisementService.LoadAd();
    }

    private void OnGameStageStateChanged(GameStageState gameStageState)
    {
        if (gameStageState == GameStageState.Advertisement)
        {
            _advertisementService.ShowAd();
        }
    }
}
