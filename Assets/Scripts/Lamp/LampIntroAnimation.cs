using UnityEngine;

public class LampIntroAnimation : MonoBehaviour, IInitializable
{
    [SerializeField] private Light _lampLight;
    [SerializeField] private AnimationCurve _animCurve;
    [SerializeField] private LampHealthBar _lampHealthBar;
    private Material _lampMaterial;
   
    [Header("New Lamp :")]
    [SerializeField] private LampEmissionController _lampEmissionController;
    [SerializeField] private AnimationCurve _lampIntensityAnimCurve;
    [SerializeField] private AnimationCurve _lampNoiseAmountAnimCurve;
    
    
    
    
    
    
    private bool _isPlaying = false;
    private float _localTime = 0;
    private float _duration;
    
    private float _lightNeutralIntensity;
    private float _currentHealthNormalized; 
    
    public void Initialize()
    {
        _lampMaterial = GetComponent<MeshRenderer>().sharedMaterial;
    }

    public void Play(float duration, int currentHealth, int maxHealth, float lightNeutralIntensity)
    {
        _duration = duration;
        _currentHealthNormalized = (float)currentHealth / maxHealth;
        _lightNeutralIntensity = lightNeutralIntensity;
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
                // OLD LAMP
                _lampHealthBar.UpdateHealth(_currentHealthNormalized, 2);
                
                _lampMaterial.SetFloat("_EnableAnimation", 1);
                _lampMaterial.SetFloat("_attackPower", 1);
                _lampLight.intensity = _lightNeutralIntensity;
                
                // NEW LAMP
                _lampEmissionController.Intensity = _lampIntensityAnimCurve.Evaluate(1);
                _lampEmissionController.BlockedModeMix = _lampNoiseAmountAnimCurve.Evaluate(1);
                
                return;
            }
            
            // OLD LAMP
            float phaseAnimated = _animCurve.Evaluate(phase);
            float enablePhase = Mathf.Clamp(phaseAnimated * 1.25f, 0f, 1f);
            float attackPowerPhase = Mathf.Clamp((phaseAnimated - 0.75f) * 4, 0f, 1f);
            float lightIntensity = Mathf.Lerp(0, _lightNeutralIntensity, phaseAnimated);
            
            float health = Mathf.Lerp(0, _currentHealthNormalized, phaseAnimated);
            _lampHealthBar.UpdateHealth(health, 2);
            
            _lampMaterial.SetFloat("_EnableAnimation", enablePhase);
            _lampMaterial.SetFloat("_attackPower", attackPowerPhase);
            _lampLight.intensity = lightIntensity;
            
            // NEW LAMP
            
            _lampEmissionController.Intensity = _lampIntensityAnimCurve.Evaluate(phase);
            _lampEmissionController.BlockedModeMix = _lampNoiseAmountAnimCurve.Evaluate(phase);
            
            _localTime += Time.deltaTime;    
        }
    }
}
