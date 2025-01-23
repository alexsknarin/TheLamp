using System;
using UnityEngine;

public abstract class AdvertisementBaseService : IAdvertisementService, IInitializable, IDisposable
{
    public abstract event Action AdSuccessfullyFinished;
    public abstract void Initialize();
    public abstract void Dispose();
    public abstract void LoadAd();
    public abstract void ShowAd();
}
