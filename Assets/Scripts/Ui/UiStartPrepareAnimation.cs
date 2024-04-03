using TMPro;
using UnityEngine;

public class UiStartPrepareAnimation : MonoBehaviour
{
    [SerializeField] private UiText _waveText;
    [SerializeField] private GameObject _upgradeButtonsPanel;
    [SerializeField] private UiUpgradePoints _uiUpgradePoints;
    [SerializeField] private GameObject _upgradeHintsPanel;
    
    private bool _isUpgradeHintsPanelShown = false;
    

    public void Play(float duration, int waveNum, LampStatsManager lampStatsManager)
    {
        _waveText.ShowWaveText("Start Wave " + waveNum.ToString());
        // CHeck if upgrades are available
        if (lampStatsManager.UpgradePoints > 0)
        {
            _upgradeButtonsPanel.SetActive(true); // Add Animation
            _uiUpgradePoints.ShowUpgradePoints(lampStatsManager.UpgradePoints);
            
            if (!_isUpgradeHintsPanelShown)
            {
                _upgradeHintsPanel.SetActive(true);
                _isUpgradeHintsPanelShown = true;
            }
            else
            {
                _upgradeHintsPanel.SetActive(false);
            }
        }

    }
}
