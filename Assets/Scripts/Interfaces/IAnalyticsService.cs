using System;

public interface IAnalyticsService
{
    public event Action OnConsentAddressedEvent;
    public void Initialize();
    public void SetConsentData(bool isConsentGiven);
    public void UpdateCollectionBehavior(bool isConsentGiven);
}
