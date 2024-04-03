
using UnityEngine;
using UnityEngine.UI;

public class UiGameOverAnimation : MonoBehaviour
{
    [SerializeField] private GameObject _gameOverButtonsGroup;
    [SerializeField] private Image _fadeImage;
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private UiText _gameOverText;
    
    
    private bool _isPlaying = false;
    private float _localTime;
    private float _duration = 5.5f;

    private Color _fadeColor1 = new Color(0, 0, 0, 1);
    private Color _fadeColor2 = new Color(0, 0, 0, 0);
    
    public void Play(float duration)
    {
        _duration = duration;
        _localTime = 0;
        _isPlaying = true;
        _fadeImage.color = _fadeColor2;
        _fadeImage.gameObject.SetActive(true);
        
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
            _fadeImage.color = Color.Lerp(_fadeColor2, _fadeColor1, phase);
            _localTime += Time.deltaTime;
        }
    }
}
