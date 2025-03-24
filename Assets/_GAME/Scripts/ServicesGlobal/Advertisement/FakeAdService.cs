using System;
using UnityEngine;

namespace _GAME.Scripts.ServicesGlobal.Advertisement
{
    public class FakeAdService: AdvertisementBaseService
    {
        private FakeAd _fakeAd;

        public FakeAdService(FakeAd fakeAd)
        {
            _fakeAd = fakeAd;
            _fakeAd.AdFinished += OnAdFinished;
        }

        public override void Dispose()
        {
            _fakeAd.AdFinished -= OnAdFinished;
        }

        public override event Action AdSuccessfullyFinished;

        public override void Initialize()
        {
            Debug.Log("AdService: Start Connecting to the Ad Service");
            // Subscribe to Connected event
            // Restart Connection if failed to connect
            // Subscribe to AdLoaded event
            // Subscribe to AdLoadedFail event
            // If load failed try again
        }

        public override void LoadAd()
        {
            Debug.Log("AdService: Start Loading Ad");
        }

        public override void ShowAd()
        {
            // If connected
            // If Ad is loaded
            _fakeAd.Play();
            // Otherwise play promo
        }

        private void OnAdFinished()
        {
            Debug.Log("AdService: Ad Finished");
            AdSuccessfullyFinished?.Invoke();
        }
    }
}
