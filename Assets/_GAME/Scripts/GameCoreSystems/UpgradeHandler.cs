using _GAME.Scripts.GameCoreSystems.DataManagement.DataTypes;

namespace _GAME.Scripts.GameCoreSystems
{
    public class UpgradeHandler
    {
        public int GetUpgradePointsAndUpdateScoreData(ref UpgradeData upgradeData)
        {
            int upgradePoints = 0;
            int newScores = upgradeData.Score - upgradeData.UsedScore;
            while (newScores >= upgradeData.CurrentScoreUpgradePrice)
            {
                upgradePoints++;
                upgradeData.UsedScore += upgradeData.CurrentScoreUpgradePrice;
                upgradeData.CurrentScoreUpgradePrice += upgradeData.CurrentScoreUpgradePriceIncrement;
                upgradeData.CurrentScoreUpgradePriceIncrement++;
                newScores = upgradeData.Score - upgradeData.UsedScore;
            }
            return upgradePoints;
        }
    }
}
