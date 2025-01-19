using System;
using UnityEngine;

public class GameOverOutGameStageAnimationController : MonoBehaviour, IInitializable
{
    [Header("UI")]
    [SerializeField] private GameObject _gameOverUi;
    [SerializeField] private TextFader _gameOverText;
    [SerializeField] private AnimationCurve _gameOverButtonsAnimationCurve;
    [SerializeField] private FadableButtonPresentation _restartWithAdButton;
    [SerializeField] private FadableButtonPresentation _restartNoAdButton;
    [SerializeField] private FadableButtonPresentation _exitButton;
    private bool _skip = false;   
    private float _duration;

    private float _localTime;
    
    public event Action GameoverOutFinished;

    public void Initialize()
    {
        enabled = false;
    }

    public void Play(bool skip, float duration)
    {
        _skip = skip;
        _duration = duration;
        _localTime = 0;
        enabled = true;
    }

    private void Update()
    {
        float phase = _localTime / _duration;
        if (_localTime >= _duration)
        {
            _gameOverUi.SetActive(false);
            enabled = false;
            GameoverOutFinished?.Invoke();
        }
        
        _gameOverText.SetVisibilityLevel(1-phase);
        _restartWithAdButton.SetVisibilityLevel(_gameOverButtonsAnimationCurve.Evaluate(phase));
        _restartNoAdButton.SetVisibilityLevel(_gameOverButtonsAnimationCurve.Evaluate(phase));
        _exitButton.SetVisibilityLevel(_gameOverButtonsAnimationCurve.Evaluate(phase));
        
        _localTime += Time.deltaTime;
    }
}
