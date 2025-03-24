using System;

namespace _GAME.Scripts.Lib.Interfaces
{
    public interface IGameSettingsService
    {
        public bool IsConsentSet { get; }
        public bool IsDataCollectionEnabled { get; }
        public event Action<bool> IsConsentSetChanged;
        public event Action<bool> IsDataCollectionEnabledChanged;
    }
}
