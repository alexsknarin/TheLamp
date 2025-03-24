using System;
using _GAME.Scripts.Lib.Interfaces;
using IDisposable = _GAME.Scripts.Lib.Interfaces.IDisposable;

namespace _GAME.Scripts.ServicesGlobal.Advertisement
{
    public abstract class AdvertisementBaseService : IAdvertisementService, IInitializable, IDisposable
    {
        public abstract event Action AdSuccessfullyFinished;
        public abstract void Initialize();
        public abstract void Dispose();
        public abstract void LoadAd();
        public abstract void ShowAd();
    }
}
