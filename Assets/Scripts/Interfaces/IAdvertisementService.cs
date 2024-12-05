using System;

public interface IAdvertisementService
{
    public event Action OnAdFinishedEvent;
    public void Initialize();
    public void ShowAd();
}
