using UnityEngine;

public class LampIntroAnimation : MonoBehaviour
{
    [SerializeField] private AnimationCurve _animCurve;
    [SerializeField] private LampHealthBar _lampHealthBar;
    [SerializeField] private LampEmissionController _lampEmissionController;
    [SerializeField] private AnimationCurve _lampIntensityAnimCurve;
    [SerializeField] private AnimationCurve _lampNoiseAmountAnimCurve;
    
    
    private bool _isPlaying = false;
    private float _localTime = 0;
    private float _duration;
    
    private int _currentHealth;
    private float _currentHealthNormalized; 
    
    public void Play(float duration, int currentHealth, int maxHealth)
    {
        _duration = duration;
        _currentHealth = currentHealth;
        _currentHealthNormalized = (float)currentHealth / maxHealth;
        _isPlaying = true;
        _localTime = 0;
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
                
                return;
            }
            float phaseAnimated = _animCurve.Evaluate(phase);
            float health = Mathf.Lerp(0, _currentHealthNormalized, phaseAnimated);
            _lampHealthBar.UpdateHealth(health, 2);
            _lampEmissionController.Intensity = _lampIntensityAnimCurve.Evaluate(phase);
            _lampEmissionController.BlockedModeMix = _lampNoiseAmountAnimCurve.Evaluate(phase);
            
            _localTime += Time.deltaTime;    
        }
    }
}
