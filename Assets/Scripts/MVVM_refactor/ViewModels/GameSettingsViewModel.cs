public class GameSettingsViewModel : IInitializable, IDisposable
{
    public Observable<bool> IsConsentSetView = new Observable<bool>();
    public Observable<bool> IsDataCollectionEnabledView = new Observable<bool>();
    
    private GameSettingsModel _gameSettingsModel;
    public GameSettingsViewModel(GameSettingsModel gameSettingsModel)
    {
        _gameSettingsModel = gameSettingsModel;
        IsConsentSetView.Value = _gameSettingsModel.IsConsentSet;
        IsDataCollectionEnabledView.Value = _gameSettingsModel.IsDataCollectionEnabled;
    }
    
    public void Initialize()
    {
        _gameSettingsModel.IsConsentSetChanged += OnIsConsentSetChanged;
        _gameSettingsModel.IsDataCollectionEnabledChanged += OnIsDataCollectionEnabledChanged;
    }

    public void Dispose()
    {
        _gameSettingsModel.IsConsentSetChanged -= OnIsConsentSetChanged;
        _gameSettingsModel.IsDataCollectionEnabledChanged -= OnIsDataCollectionEnabledChanged;
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
    
    // Handle Model Events
    
    private void OnIsConsentSetChanged(bool value)
    {
        IsConsentSetView.Value = value;
    }

    private void OnIsDataCollectionEnabledChanged(bool value)
    {
        IsDataCollectionEnabledView.Value = value;
    }
}
