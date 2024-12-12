// TODO: settings as dictionary - unified interface to change them
// TODO: separate view models for different settings ???? 

using System;
using UnityEngine;

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
                OnIsConsentSetChangedEvent?.Invoke(value);
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
                OnIsDataCollectionEnabledChangedEvent?.Invoke(value);
            }
        }
    }

    public event Action<bool> OnIsConsentSetChangedEvent;
    public event Action<bool> OnIsDataCollectionEnabledChangedEvent;

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
