using System;

public interface IAdvertisementService
{
    public event Action AdSuccessfullyFinished;
    public void LoadAd();
    public void ShowAd();
}
