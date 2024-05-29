
using UnityEngine;
using UnityEngine.UI;

public class UiGameOverAnimation : MonoBehaviour
{
    [SerializeField] private GameObject _gameOverButtonsGroup;
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private UiText _gameOverText;
    private UnityEngine.Rendering.Universal.ColorAdjustments _colorAdjustments;

    private bool _isPlaying = false;
    private float _localTime;
    private float _duration = 5.5f;
    
    public void Play(float duration, UnityEngine.Rendering.Universal.ColorAdjustments colorAdjustments)
    {
        _colorAdjustments = colorAdjustments;
        _duration = duration;
        _localTime = 0;
        _isPlaying = true;
        
        _gameOverPanel.SetActive(true);
        _gameOverText.ShowWaveText("Game Over");
    }

    void Update()
    {
        if (_isPlaying)
        {
            float phase = _localTime / _duration;
            if (phase > 1)
            {
                _isPlaying = false;
                _gameOverButtonsGroup.SetActive(true);
            }
            _colorAdjustments.postExposure.Override(Mathf.Lerp(0, -10, phase));
            _localTime += Time.deltaTime;
        }
    }
}
