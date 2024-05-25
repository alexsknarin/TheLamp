using UnityEngine;

public class LampDeathAnimation : MonoBehaviour, IInitializable
{
    [SerializeField] private Light _lampLight;
    [SerializeField] private AnimationCurve _animCurve;
    [SerializeField] private LampHealthBar _lampHealthBar;
    [SerializeField] private LampEmissionController _lampEmissionController;
    private Material _lampMaterial;
    private bool _isPlaying = false;
    private float _duration;
    private float _localTime = 0;
    private float _lightNeutralIntensity;
    
    public void Initialize()
    {
        _lampMaterial = GetComponent<MeshRenderer>().sharedMaterial;
    }
    
    public void Play(float duration, float lightNeutralIntensity)
    {
        _duration = duration;
        _lightNeutralIntensity = lightNeutralIntensity;
        _localTime = 0;
        _isPlaying = true;
    }

    void Update()
    {
        if (_isPlaying)
        {
            float phase = _localTime / _duration;
            if (phase > 1)
            {
                _isPlaying = false;
                _lampMaterial.SetFloat("_EnableAnimation", 0);
                _lampMaterial.SetFloat("_attackPower", 0);
                _lampLight.intensity = 0;
                _lampHealthBar.UpdateHealth(0, 0);
                return;
            }
        
            float phaseAnimated = _animCurve.Evaluate(phase);
        
            _lampMaterial.SetFloat("_EnableAnimation", phaseAnimated);
            _lampMaterial.SetFloat("_attackPower", 0);
            _lampLight.intensity = Mathf.Lerp(_lightNeutralIntensity, 0f, 1-phaseAnimated);
            
            // NEW LAMP
            _lampEmissionController.Intensity = phaseAnimated;
            
        
            _localTime += Time.deltaTime;
        }
    }
}
