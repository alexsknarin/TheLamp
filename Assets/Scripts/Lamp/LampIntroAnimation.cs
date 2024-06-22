using System;
using UnityEngine;

public class LampIntroAnimation : MonoBehaviour
{
    [SerializeField] private AnimationCurve _animCurve;
    [SerializeField] private LampHealthBar _lampHealthBar;
    [SerializeField] private LampEmissionController _lampEmissionController;
    [SerializeField] private MeshRenderer _lampAttackZoneRenderer;
    [SerializeField] private AnimationCurve _lampIntensityAnimCurve;
    [SerializeField] private AnimationCurve _lampNoiseAmountAnimCurve;
    private Material _lampAttackZoneMaterial;

    private bool _isPlaying = false;
    private float _localTime = 0;
    private float _duration;
    
    private int _currentHealth;
    private float _currentHealthNormalized;

    private void Awake()
    {
        _lampAttackZoneMaterial = _lampAttackZoneRenderer.material;
    }

    public void Play(float duration, int currentHealth, int maxHealth)
    {
        _duration = duration;
        _currentHealth = currentHealth;
        _currentHealthNormalized = (float)currentHealth / maxHealth;
        _isPlaying = true;
        _localTime = 0;
        _lampAttackZoneRenderer.gameObject.SetActive(true);
    }
    
    void Update()
    {
        if (_isPlaying)
        {
            float phase = _localTime / _duration;
            if (phase > 1)
            {
                _isPlaying = false;
                _lampHealthBar.UpdateHealth(_currentHealthNormalized, _currentHealth);
                _lampEmissionController.Intensity = _lampIntensityAnimCurve.Evaluate(1);
                _lampEmissionController.BlockedModeMix = _lampNoiseAmountAnimCurve.Evaluate(1);
                _lampAttackZoneMaterial.SetFloat("_Alpha", 0.005f);    
                return;
            }
            float phaseAnimated = _animCurve.Evaluate(phase);
            float health = Mathf.Lerp(0, _currentHealthNormalized, phaseAnimated);
            _lampHealthBar.UpdateHealth(health, 2);
            _lampEmissionController.Intensity = _lampIntensityAnimCurve.Evaluate(phase);
            _lampEmissionController.BlockedModeMix = _lampNoiseAmountAnimCurve.Evaluate(phase);
            _lampAttackZoneMaterial.SetFloat("_Alpha", Mathf.Lerp(0, 0.005f, _lampIntensityAnimCurve.Evaluate(phase)));
            
            _localTime += Time.deltaTime;    
        }
    }
}
