using UnityEngine;

public class UiStartFightAnimation : MonoBehaviour
{
    [SerializeField] private UiText _waveText;
    [SerializeField] private GameObject _upgradeButtonsPanel;    

    public void Play(float duration)
    {
        _waveText.HideWaveText();
        _upgradeButtonsPanel.SetActive(false); // Add Animation
    }
}
