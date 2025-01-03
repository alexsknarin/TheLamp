using System;

public class GameSettingsService: IGameSettingsService, IInitializable, IDisposable
{
    private IGameSettingsProviderService _gameSettingsProviderService;
    private GameSettingsModel _gameSettingsModel;

    public GameSettingsService(IGameSettingsProviderService gameSettingsProviderService, GameSettingsModel gameSettingsModel)
    {
        _gameSettingsModel = gameSettingsModel;
        _gameSettingsProviderService = gameSettingsProviderService;
    }
    public event Action<bool> IsConsentSetChanged;
    public event Action<bool> IsDataCollectionEnabledChanged;
    
    public bool IsConsentSet => _gameSettingsModel.IsConsentSet;
    public bool IsDataCollectionEnabled => _gameSettingsModel.IsDataCollectionEnabled;

    public void Initialize()
    {
        _gameSettingsModel.IsConsentSetChanged += HandleConsentSetOnChangedEvent;
        _gameSettingsModel.IsDataCollectionEnabledChanged += HandleDataCollectionEnabledChanged;
    }

    public void Dispose()
    {
        _gameSettingsModel.IsConsentSetChanged += HandleConsentSetOnChangedEvent;
        _gameSettingsModel.IsDataCollectionEnabledChanged += HandleDataCollectionEnabledChanged;
    }

    private void Save()
    {
        _gameSettingsProviderService.Save();
    }

    private void HandleConsentSetOnChangedEvent(bool value)
    {
        Save();
        IsConsentSetChanged?.Invoke(value);
    }

    private void HandleDataCollectionEnabledChanged(bool value)
    {
        Save();
        IsDataCollectionEnabledChanged?.Invoke(value);
    }
}
