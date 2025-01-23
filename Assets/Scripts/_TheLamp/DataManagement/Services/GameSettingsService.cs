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
        _gameSettingsModel.IsConsentSetChanged += OnIsConsentSetChanged;
        _gameSettingsModel.IsDataCollectionEnabledChanged += OnIsDataCollectionEnabledChanged;
    }

    public void Dispose()
    {
        _gameSettingsModel.IsConsentSetChanged += OnIsConsentSetChanged;
        _gameSettingsModel.IsDataCollectionEnabledChanged += OnIsDataCollectionEnabledChanged;
    }

    private void Save()
    {
        _gameSettingsProviderService.Save();
    }
    
    // Event Handle Methods
    private void OnIsConsentSetChanged(bool value)
    {
        Save();
        IsConsentSetChanged?.Invoke(value);
    }

    private void OnIsDataCollectionEnabledChanged(bool value)
    {
        Save();
        IsDataCollectionEnabledChanged?.Invoke(value);
    }
}
