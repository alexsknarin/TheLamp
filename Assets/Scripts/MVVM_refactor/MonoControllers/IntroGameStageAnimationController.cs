using System;
using UnityEngine;
using UnityEngine.Rendering;

public class IntroGameStageAnimationController : MonoBehaviour, IInitializable
{
    [Header("Settings")]
    [SerializeField] private bool _skip = false;   
    [SerializeField] private float _duration;
    [SerializeField] private float _cameraStartZPosition = -7.1f;
    [SerializeField] private float _cameraEndZPosition = -5.88f;
    [Header("Scene Dependencies")]
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private AnimationCurve _cameraAnimationCurve;
    [SerializeField] private Volume _postProcessingVolume;
    [Header("Lamp Dependencies")]
    [SerializeField] private AnimationCurve _animCurve;
    [SerializeField] private LampHealthBarController _lampHealthBarController;
    [SerializeField] private LampEmissionController _lampEmissionController;
    [SerializeField] private MeshRenderer _lampAttackZoneRenderer;
    [SerializeField] private AnimationCurve _lampIntensityAnimCurve;
    [SerializeField] private AnimationCurve _lampNoiseAmountAnimCurve;
    private Material _lampAttackZoneMaterial;
    
    private UnityEngine.Rendering.Universal.ColorAdjustments _colorAdjustments;

    private float _localTime;
    private bool _isPlaying;
    private float _currentHealth;
    
    public event Action OnFinishedEvent;

    public void Initialize()
    {
        // Get Control over Exposure
        VolumeProfile volumeProfile = _postProcessingVolume.profile;
        if (!volumeProfile) throw new System.NullReferenceException(nameof(UnityEngine.Rendering.VolumeProfile));
        // You can leave this variable out of your function, so you can reuse it throughout your class.
        if (!volumeProfile.TryGet(out _colorAdjustments))
            throw new System.NullReferenceException(nameof(_colorAdjustments));
        _colorAdjustments.postExposure.Override(-15);
        _lampAttackZoneMaterial = _lampAttackZoneRenderer.material;

        _isPlaying = false;
    }
    
    public void Play(float normalizedHealth)
    {
        if (_skip)
        {
            SetFinalState();
            return;
        }
        _currentHealth = normalizedHealth;
        _lampAttackZoneRenderer.gameObject.SetActive(true);
        
        _localTime = 0;
        _isPlaying = true;
    }
    
    void Update()
    {
        if (_isPlaying)
        {
            float phase = _localTime / _duration;
            Vector3 cameraPosition = _cameraTransform.position;
            if (phase > 1)
            {
                SetFinalState();
            }
            _colorAdjustments.postExposure.Override(Mathf.Lerp(-8, 0, phase));
            cameraPosition.z = Mathf.Lerp(_cameraStartZPosition, _cameraEndZPosition, _cameraAnimationCurve.Evaluate(phase));
            _cameraTransform.position = cameraPosition;
            
            float phaseAnimated = _animCurve.Evaluate(phase);
            float health = Mathf.Lerp(0, _currentHealth, phaseAnimated);
            _lampHealthBarController.SetHealth(health);
            _lampEmissionController.Intensity = _lampIntensityAnimCurve.Evaluate(phase);
            _lampEmissionController.BlockedModeMix = _lampNoiseAmountAnimCurve.Evaluate(phase);
            _lampAttackZoneMaterial.SetFloat("_Alpha", Mathf.Lerp(0, 0.005f, _lampIntensityAnimCurve.Evaluate(phase)));
            
            _localTime += Time.deltaTime;
        }
    }

    private void SetFinalState()
    {
        _isPlaying = false;
        _localTime = 0;
        Vector3 cameraPosition = _cameraTransform.position;
        cameraPosition.z = _cameraEndZPosition;
        _cameraTransform.position = cameraPosition;
        _colorAdjustments.postExposure.Override(0);
        // Lamp
        _lampHealthBarController.SetHealth(_currentHealth);
        _lampEmissionController.Intensity = _lampIntensityAnimCurve.Evaluate(1);
        _lampEmissionController.BlockedModeMix = _lampNoiseAmountAnimCurve.Evaluate(1);
        _lampAttackZoneMaterial.SetFloat("_Alpha", 0.005f);    
        
        OnFinishedEvent?.Invoke();
    }
    
}
