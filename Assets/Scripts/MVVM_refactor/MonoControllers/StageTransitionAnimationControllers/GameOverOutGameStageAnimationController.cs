using System;
using UnityEngine;

public class GameOverOutGameStageAnimationController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool _skip = false;   
    [SerializeField] private float _duration;
    [Header("UI")]
    [SerializeField] private GameObject _gameOverUi;
    [SerializeField] private TextFader _gameOverText;
    [SerializeField] private AnimationCurve _gameOverButtonsAnimationCurve;
    [SerializeField] private FadableButtonPresentation _restartWithAdButton;
    [SerializeField] private FadableButtonPresentation _restartNoAdButton;
    [SerializeField] private FadableButtonPresentation _exitButton;
    
    public event Action GameoverOutFinished;
    
    private float _localTime;
    private bool _isPlaying;

    public void Play()
    {
        _localTime = 0;
        _isPlaying = true;
    }
    
    private void Update()
    {
        if (_isPlaying)
        {
            float phase = _localTime / _duration;
            if (_localTime >= _duration)
            {
                _gameOverUi.SetActive(false);
                _isPlaying = false;
                GameoverOutFinished?.Invoke();
            }
            
            _gameOverText.SetVisibilityLevel(1-phase);
            _restartWithAdButton.SetVisibilityLevel(_gameOverButtonsAnimationCurve.Evaluate(phase));
            _restartNoAdButton.SetVisibilityLevel(_gameOverButtonsAnimationCurve.Evaluate(phase));
            _exitButton.SetVisibilityLevel(_gameOverButtonsAnimationCurve.Evaluate(phase));
            
            _localTime += Time.deltaTime;
            
        }
    }
}
