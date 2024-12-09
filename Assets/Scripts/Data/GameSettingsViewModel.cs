public class GameSettingsViewModel : IInitializable, IDisposable
{   
    private GameSettingsModel _gameSettingsModel;
    
    public Observable<bool> IsConsentSetView = new Observable<bool>();
    public Observable<bool> IsDataCollectionEnabledView = new Observable<bool>();
    
    public GameSettingsViewModel(GameSettingsModel gameSettingsModel)
    {
        _gameSettingsModel = gameSettingsModel;
        IsConsentSetView.Value = _gameSettingsModel.IsConsentSet;
        IsDataCollectionEnabledView.Value = _gameSettingsModel.IsDataCollectionEnabled;
    }
    
    public void Initialize()
    {
        _gameSettingsModel.OnIsConsentSetChangedEvent += HandleConsentSetOnChangedEvent;
        _gameSettingsModel.OnIsDataCollectionEnabledChangedEvent += HandleDataCollectionEnabledChanged;
    }

    public void Dispose()
    {
        _gameSettingsModel.OnIsConsentSetChangedEvent -= HandleConsentSetOnChangedEvent;
        _gameSettingsModel.OnIsDataCollectionEnabledChangedEvent -= HandleDataCollectionEnabledChanged;
    }
    
    // Handle Model Events
    
    private void HandleConsentSetOnChangedEvent(bool value)
    {
        IsConsentSetView.Value = value;
    }

    private void HandleDataCollectionEnabledChanged(bool value)
    {
        IsDataCollectionEnabledView.Value = value;
    }
    
    // Handle View Events
    public void HandleYesInitialConsentButtonClicked()
    {
        _gameSettingsModel.AnalyticsConsentSet(true);
    }
    
    public void HandleNoInitialConsentButtonClicked()
    {
        _gameSettingsModel.AnalyticsConsentSet(false);
    }
    
    public void HandleEnableDataCollectionButtonClicked()
    {
        _gameSettingsModel.AnalyticsConsentSet(true);
    }
    
    public void HandleDisableDataCollectionButtonClicked()
    {
        _gameSettingsModel.AnalyticsConsentSet(false);
    }
    
}
