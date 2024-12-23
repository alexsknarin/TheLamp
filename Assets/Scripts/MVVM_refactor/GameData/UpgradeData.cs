[System.Serializable]
public struct UpgradeData
{
    public int Score;
    public int UsedScore;
    // How much scores player needs to upgrade
    public int CurrentScoreUpgradePrice;
    // How much will price increase after upgrade
    public int CurrentScoreUpgradePriceIncrement;
}
