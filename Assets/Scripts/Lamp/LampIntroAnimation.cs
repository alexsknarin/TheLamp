using UnityEngine;

public class LampIntroAnimation : MonoBehaviour, IInitializable
{
    [SerializeField] private Light _lampLight;
    [SerializeField] private AnimationCurve _animCurve;
    [SerializeField] private LampHealthBar _lampHealthBar;
    private Material _lampMaterial;
    
    private bool _isPlaying = false;
    private float _localTime = 0;
    private float _duration;
    
    private float _initialHealth;
    private float _lightNeutralIntensity;
    
    public void Initialize()
    {
        _lampMaterial = GetComponent<MeshRenderer>().sharedMaterial;
    }

    public void Play(float duration, float initialHealth, float lightNeutralIntensity)
    {
        _duration = duration;
        _initialHealth = initialHealth;
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
                _lampMaterial.SetFloat("_EnableAnimation", 1);
                _lampMaterial.SetFloat("_attackPower", 1);
                _lampLight.intensity = _lightNeutralIntensity;
                _lampHealthBar.UpdateHealth(_initialHealth, 2);
                return;
            }
            float phaseAnimated = _animCurve.Evaluate(phase);
            float enablePhase = Mathf.Clamp(phaseAnimated * 1.25f, 0f, 1f);
            float attackPowerPhase = Mathf.Clamp((phaseAnimated - 0.75f) * 4, 0f, 1f);
            float lightIntensity = Mathf.Lerp(0, _lightNeutralIntensity, phaseAnimated);
            float health = Mathf.Lerp(0, _initialHealth, phaseAnimated);
        
            _lampHealthBar.UpdateHealth(health, 2);
            _lampMaterial.SetFloat("_EnableAnimation", enablePhase);
            _lampMaterial.SetFloat("_attackPower", attackPowerPhase);
            _lampLight.intensity = lightIntensity;
            _localTime += Time.deltaTime;    
        }
    }
}
