using System;

public class GameSettingsService: IGameSettingsService, IInitializable, IDisposable
{
    private IGameSettingsProvider _gameSettingsProvider;
    private GameSettingsModel _gameSettingsModel;
    
    public bool IsConsentSet => _gameSettingsModel.IsConsentSet;
    public bool IsDataCollectionEnabled => _gameSettingsModel.IsDataCollectionEnabled;
    public event Action<bool> OnIsConsentSetChangedEvent;
    public event Action<bool> OnIsDataCollectionEnabledChangedEvent;
    
    public GameSettingsService(IGameSettingsProvider gameSettingsProvider, GameSettingsModel gameSettingsModel)
    {
        _gameSettingsModel = gameSettingsModel;
        _gameSettingsProvider = gameSettingsProvider;
    }

    public void Initialize()
    {
        _gameSettingsModel.OnIsConsentSetChangedEvent += HandleConsentSetOnChangedEvent;
        _gameSettingsModel.OnIsDataCollectionEnabledChangedEvent += HandleDataCollectionEnabledChanged;
    }

    public void Dispose()
    {
        _gameSettingsModel.OnIsConsentSetChangedEvent += HandleConsentSetOnChangedEvent;
        _gameSettingsModel.OnIsDataCollectionEnabledChangedEvent += HandleDataCollectionEnabledChanged;
    }

    private void Save()
    {
        _gameSettingsProvider.Save();
    }

    private void HandleConsentSetOnChangedEvent(bool value)
    {
        Save();
        OnIsConsentSetChangedEvent?.Invoke(value);
    }

    private void HandleDataCollectionEnabledChanged(bool value)
    {
        Save();
        OnIsDataCollectionEnabledChangedEvent?.Invoke(value);
    }
}
