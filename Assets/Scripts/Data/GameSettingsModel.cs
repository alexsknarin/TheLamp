// TODO: settings as dictionary - unified interface to change them
// TODO: separate view models for different settings ???? 
using UnityEngine;

public class GameSettingsModel
{
    private IGameSettingsService _gameSettingsService;
    private GameSettings _gameSettings;
    
    public Observable<bool> IsConsentSet = new Observable<bool>();
    public Observable<bool> IsDataCollectionEnabled = new Observable<bool>();
    
    public GameSettingsModel(GameSettings gameSettings, IGameSettingsService gameSettingsService)
    {
        _gameSettings = gameSettings;
        _gameSettingsService = gameSettingsService;
        
        IsConsentSet.Value = _gameSettings.IsConsentSet;
        IsDataCollectionEnabled.Value = _gameSettings.IsDataCollectionEnabled;
    }
    
    public void AnalyticsConsentSet(bool value)
    {
        IsConsentSet.Value = true;
        _gameSettings.IsConsentSet = IsConsentSet.Value;

        IsDataCollectionEnabled.Value = value;
        _gameSettings.IsDataCollectionEnabled = IsDataCollectionEnabled.Value;
        
        _gameSettingsService.Save();
    }
}
