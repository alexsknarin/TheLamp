// TODO: settings as dictionary - unified interface to change them
// TODO: separate view models for different settings ???? 

using System;

public class GameSettingsModel
{
    private GameSettings _gameSettings;

    public bool IsConsentSet
    {
        get => _gameSettings.IsConsentSet;
        private set
        {
            var oldValue = _gameSettings.IsConsentSet;
            _gameSettings.IsConsentSet = value;
            if (!oldValue.Equals(value))
            {
                IsConsentSetChanged?.Invoke(value);
            }
        }
    }

    public bool IsDataCollectionEnabled
    {
        get => _gameSettings.IsDataCollectionEnabled;
        private set
        {
            var oldValue = _gameSettings.IsDataCollectionEnabled;
            _gameSettings.IsDataCollectionEnabled = value;
            if (!oldValue.Equals(value))
            {
                IsDataCollectionEnabledChanged?.Invoke(value);
            }
        }
    }

    public event Action<bool> IsConsentSetChanged;
    public event Action<bool> IsDataCollectionEnabledChanged;

    public GameSettingsModel(GameSettings gameSettings)
    {
        _gameSettings = gameSettings;
    }

    public void AnalyticsConsentSet(bool value)
    {
        IsConsentSet = true;
        IsDataCollectionEnabled = value;
    }
}
