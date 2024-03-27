using TMPro;
using UnityEngine;

public class WaveText : MonoBehaviour
{
    [SerializeField] private TMP_Text _waveText;

    public void ShowWaveText(int wave)
    {
        _waveText.text = "Start Wave " + wave.ToString();
        _waveText.enabled = true;
    }
    
    public void HideWaveText()
    {
        _waveText.enabled = false;
    }
}
