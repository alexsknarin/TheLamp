using System;

public interface IAnalyticsService
{
    public event Action OnConsentAddressedEvent;
    public void SubmitWaveStartEvent(int wave);
    public void SubmitWaveEndEvent(int wave);
    public void SubmitLampDamageEvent(EnemyBase enemy);
    public void SubmitHealthUpgradeEvent();
    public void SubmitCoolUpgradeEvent();
}
