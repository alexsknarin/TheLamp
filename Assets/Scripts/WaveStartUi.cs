using TMPro;
using UnityEngine;

public class WaveStartUi : MonoBehaviour
{
    [SerializeField] private TMP_Text _waveText;

    private void OnEnable()
    {
        EnemyManager.OnWavePrepared += PrepareWave;
        EnemyManager.OnWaveStarted += StartWave;
    }

    private void OnDisable()
    {
        EnemyManager.OnWavePrepared -= PrepareWave;
        EnemyManager.OnWaveStarted -= StartWave;
    }
    

    private void PrepareWave(int wave)
    {
        _waveText.text = "Start Wave " + wave.ToString();
        _waveText.enabled = true;
    }
    
    private void StartWave()
    {
        _waveText.enabled = false;
    }
}
