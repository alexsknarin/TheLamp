using System;
using UnityEngine;
using UnityEngine.Rendering;

public class GameOverInGameStageAnimationController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool _skip = false;   
    [SerializeField] private float _duration;
    [SerializeField] private float _cameraStartZPosition = -5.88f;
    [SerializeField] private float _cameraEndZPosition = -7.1f;
    [SerializeField] private float _startExposure = 0;
    [SerializeField] private float _endExposure = -8;
    [Header("Scene Dependencies")]
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private AnimationCurve _cameraAnimationCurve;
    [SerializeField] private Volume _postProcessingVolume;
    [SerializeField] private GameObject _gameOverUi;
    [SerializeField] private AnimationCurve _gameOverTextAnimationCurve;
    [SerializeField] private TextFader _gameOverText;
    [SerializeField] private AnimationCurve _gameOverButtonsAnimationCurve;
    [SerializeField] private FadableButtonPresentation _restartWithAdButton;
    [SerializeField] private FadableButtonPresentation _restartNoAdButton;
    [SerializeField] private FadableButtonPresentation _exitButton;
    
    public event Action OnFinishedEvent;
    
    private UnityEngine.Rendering.Universal.ColorAdjustments _colorAdjustments;
    private float _localTime;
    private bool _isPlaying;
    
    public void Initialize()
    {
        // Get Control over Exposure
        VolumeProfile volumeProfile = _postProcessingVolume.profile;
        if (!volumeProfile) throw new NullReferenceException(nameof(VolumeProfile));
        // You can leave this variable out of your function, so you can reuse it throughout your class.
        if (!volumeProfile.TryGet(out _colorAdjustments))
            throw new NullReferenceException(nameof(_colorAdjustments));
        _colorAdjustments.postExposure.Override(_startExposure);
        _isPlaying = false;
    }
    
    public void Play()
    {
        if (_skip)
        {
            SetFinalState();
            return;
        }
        
        _gameOverUi.SetActive(true);
        _gameOverText.SetVisibilityLevel(0);
        _restartWithAdButton.SetVisibilityLevel(0);
        _restartNoAdButton.SetVisibilityLevel(0);
        _exitButton.SetVisibilityLevel(0);
        
        
        _localTime = 0;
        _isPlaying = true;

    }

    private void Update()
    {
        if (_isPlaying)
        {
            float phase = _localTime / _duration;
            if (phase > 1)
            {
                SetFinalState();
            }
            
            Vector3 cameraPosition = _cameraTransform.position;
            cameraPosition.z = Mathf.Lerp(_cameraStartZPosition, _cameraEndZPosition, _cameraAnimationCurve.Evaluate(phase));
            _cameraTransform.position = cameraPosition;
            _colorAdjustments.postExposure.Override(Mathf.Lerp(_startExposure, _endExposure, phase));
            
            _gameOverText.SetVisibilityLevel(_gameOverTextAnimationCurve.Evaluate(phase));
            
            _restartWithAdButton.SetVisibilityLevel(_gameOverButtonsAnimationCurve.Evaluate(phase));
            _restartNoAdButton.SetVisibilityLevel(_gameOverButtonsAnimationCurve.Evaluate(phase));
            _exitButton.SetVisibilityLevel(_gameOverButtonsAnimationCurve.Evaluate(phase));
            
            
            _localTime += Time.deltaTime;
        }
    }

    private void SetFinalState()
    {
        _isPlaying = false;
        _localTime = 0;
        
        // Environment
        Vector3 cameraPosition = _cameraTransform.position;
        cameraPosition.z = _cameraEndZPosition;
        _cameraTransform.position = cameraPosition;
        _colorAdjustments.postExposure.Override(_endExposure);
        //UI
        _gameOverText.SetVisibilityLevel(1);
        _restartWithAdButton.SetVisibilityLevel(1);
        _restartNoAdButton.SetVisibilityLevel(1);
        _exitButton.SetVisibilityLevel(1);
        
        OnFinishedEvent?.Invoke();
    }
}
