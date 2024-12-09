public class GameSettingsViewModel : IInitializable, IDisposable
{   
    private GameSettingsModel _gameSettingsModel;
    
    public Observable<bool> IsConsentSetView = new Observable<bool>();
    public Observable<bool> IsDataCollectionEnabledView = new Observable<bool>();
    
    public GameSettingsViewModel(GameSettingsModel gameSettingsModel)
    {
        _gameSettingsModel = gameSettingsModel;
        IsConsentSetView.Value = _gameSettingsModel.IsConsentSet.Value;
        IsDataCollectionEnabledView.Value = _gameSettingsModel.IsDataCollectionEnabled.Value;
    }
    
    public void Initialize()
    {
        _gameSettingsModel.IsConsentSet.OnChangedEvent += HandleConsentSetOnChangedEvent;
        _gameSettingsModel.IsDataCollectionEnabled.OnChangedEvent += HandleDataCollectionEnabledChanged;
    }

    public void Dispose()
    {
        _gameSettingsModel.IsConsentSet.OnChangedEvent -= HandleConsentSetOnChangedEvent;
        _gameSettingsModel.IsDataCollectionEnabled.OnChangedEvent -= HandleDataCollectionEnabledChanged;
    }
    
    // Handle Model Events
    
    private void HandleConsentSetOnChangedEvent(object sender, Observable<bool>.ChangedEventArgs e)
    {
        IsConsentSetView.Value = e.NewValue;
    }

    private void HandleDataCollectionEnabledChanged(object sender, Observable<bool>.ChangedEventArgs e)
    {
        IsDataCollectionEnabledView.Value = e.NewValue;
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
