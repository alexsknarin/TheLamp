using System;

namespace _GAME.Scripts.Lib.Interfaces
{
    public interface IAnalyticsService
    {
        public event Action ConsentAddressed;
        public void SubmitWaveStartEvent(int wave);
        public void SubmitWaveEndEvent(int wave);
        public void SubmitLampDamageEvent(string enemyTypeName);
        public void SubmitHealthUpgradeEvent();
        public void SubmitCoolUpgradeEvent();
        public void SubmitAttackUpgradeEvent();
    }
}
