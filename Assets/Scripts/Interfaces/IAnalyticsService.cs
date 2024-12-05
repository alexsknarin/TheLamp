public interface IAnalyticsService
{
    public void Initialize();
    public void SetConsentData(bool isConsentGiven);
    public void UpdateCollectionBehavior(bool isConsentGiven);
    public void StartAnalyticsCollection();
    public void StopAnalyticsCollection();
}
