using System;

public interface IGameSettingsService
{
    public bool IsConsentSet { get; }
    public bool IsDataCollectionEnabled { get; }
    public event Action<bool> OnIsConsentSetChangedEvent;
    public event Action<bool> OnIsDataCollectionEnabledChangedEvent;
}
