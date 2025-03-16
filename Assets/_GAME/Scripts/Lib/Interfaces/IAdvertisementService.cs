using System;

namespace _GAME.Scripts.Lib.Interfaces
{
    public interface IAdvertisementService
    {
        public event Action AdSuccessfullyFinished;
        public void LoadAd();
        public void ShowAd();
    }
}
