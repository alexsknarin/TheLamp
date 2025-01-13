using System;
using UnityEngine;
using UnityEngine.Rendering;

public class IntroGameStageAnimationController : MonoBehaviour, IInitializable
{
    [Header("Settings")]
    [SerializeField] private float _cameraStartZPosition = -7.1f;
    [SerializeField] private float _cameraEndZPosition = -5.88f;
    [SerializeField] private float _startExposure = -8;
    [SerializeField] private float _endExposure = 0;
    [Header("Scene Dependencies")]
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private AnimationCurve _cameraAnimationCurve;
    [SerializeField] private Volume _postProcessingVolume;
    [Header("Lamp Dependencies")]
    [SerializeField] private GameObject _lampGlassObject;
    [SerializeField] private GameObject _lampFracturedGlassObject;
    [SerializeField] private AnimationCurve _animCurve;
    [SerializeField] private LampHealthBarController _lampHealthBarController;
    [SerializeField] private LampEmissionController _lampEmissionController;
    [SerializeField] private MeshRenderer _lampAttackZoneRenderer;
    [SerializeField] private AnimationCurve _lampIntensityAnimCurve;
    [SerializeField] private AnimationCurve _lampNoiseAmountAnimCurve;
    [Header("UI Dependencies")]
    [SerializeField] private GameObject _ingameUi;
    [SerializeField] private GameObject _waveText;
    [SerializeField] private AnimationCurve _uiAnimationCurve;
    [SerializeField] private FadableButtonPresentation _exitButton;
    [SerializeField] private FadableButtonPresentation _restartButton;
    [SerializeField] private FadableButtonPresentation _enableDataButton;
    [SerializeField] private FadableButtonPresentation _disableDataButton;
    [SerializeField] private GameObject _upgradeUi;
    private bool _skip = false;   
    private float _duration;
    private Material _lampAttackZoneMaterial;
    private UnityEngine.Rendering.Universal.ColorAdjustments _colorAdjustments;
    private float _localTime;
    private bool _isPlaying;
    private float _currentHealth;
    
    public event Action IntroFinished;

    public void Initialize()
    {
        // Get Control over Exposure
        VolumeProfile volumeProfile = _postProcessingVolume.profile;
        if (!volumeProfile) throw new NullReferenceException(nameof(VolumeProfile));
        // You can leave this variable out of your function, so you can reuse it throughout your class.
        if (!volumeProfile.TryGet(out _colorAdjustments))
            throw new NullReferenceException(nameof(_colorAdjustments));
        _colorAdjustments.postExposure.Override(_startExposure);
        _lampAttackZoneMaterial = _lampAttackZoneRenderer.material;
        
        _isPlaying = false;
    }
    
    public void Play(bool skip, float duration, float normalizedHealth)
    {
        _skip = skip;
        _duration = duration;
        
        _lampGlassObject.SetActive(true);
        _lampFracturedGlassObject.SetActive(false);
        
        _currentHealth = normalizedHealth;
        _lampHealthBarController.DisableLastHealth();
        _lampAttackZoneRenderer.gameObject.SetActive(true);
        _ingameUi.SetActive(true);
        _waveText.SetActive(false);
        _upgradeUi.SetActive(false);
        _exitButton.SetVisibilityLevel(0);
        _restartButton.SetVisibilityLevel(0);
        _enableDataButton.SetVisibilityLevel(0);
        _disableDataButton.SetVisibilityLevel(0);
        
        if (_skip)
        {
            SetFinalState();
            return;
        }
        
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
            // Environment  
            _colorAdjustments.postExposure.Override(Mathf.Lerp(_startExposure, _endExposure, phase));
            Vector3 cameraPosition = _cameraTransform.position;
            cameraPosition.z = Mathf.Lerp(_cameraStartZPosition, _cameraEndZPosition, _cameraAnimationCurve.Evaluate(phase));
            _cameraTransform.position = cameraPosition;
            // Lamp
            float phaseAnimated = _animCurve.Evaluate(phase);
            float health = Mathf.Lerp(0, _currentHealth, phaseAnimated);
            _lampHealthBarController.SetHealth(health);
            _lampEmissionController.Intensity = _lampIntensityAnimCurve.Evaluate(phase);
            _lampEmissionController.BlockedModeMix = _lampNoiseAmountAnimCurve.Evaluate(phase);
            _lampAttackZoneMaterial.SetFloat("_Alpha", Mathf.Lerp(0, 0.005f, _lampIntensityAnimCurve.Evaluate(phase)));
            // UI
            _exitButton.SetVisibilityLevel(_uiAnimationCurve.Evaluate(phase));
            _restartButton.SetVisibilityLevel(_uiAnimationCurve.Evaluate(phase));
            _enableDataButton.SetVisibilityLevel(_uiAnimationCurve.Evaluate(phase));
            _disableDataButton.SetVisibilityLevel(_uiAnimationCurve.Evaluate(phase));
            
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
        // Lamp
        _lampHealthBarController.SetHealth(_currentHealth);
        _lampEmissionController.Intensity = _lampIntensityAnimCurve.Evaluate(1);
        _lampEmissionController.BlockedModeMix = _lampNoiseAmountAnimCurve.Evaluate(1);
        _lampAttackZoneMaterial.SetFloat("_Alpha", 0.005f);
        // UI
        _exitButton.SetVisibilityLevel(1);
        _restartButton.SetVisibilityLevel(1);
        _enableDataButton.SetVisibilityLevel(1);
        _disableDataButton.SetVisibilityLevel(1);
        
        IntroFinished?.Invoke();
    }
    
}
