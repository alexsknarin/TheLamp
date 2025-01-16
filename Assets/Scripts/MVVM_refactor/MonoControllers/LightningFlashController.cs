using UnityEngine;

public class LightningFlashController : MonoBehaviour, IInitializable
{
    [SerializeField] private Light _light;  
    [SerializeField] private float _flashDuration;
    [SerializeField] private AnimationCurve _flashCurve;
    [SerializeField] private float _lightMaxIntensity;
    private float _localTime;
    private bool _isPlaying = false;

    public void Initialize()
    {
        _light.enabled = false;
    }

    public void Play()
    {
        _light.enabled = true;
        _light.intensity = 0;
        _localTime = 0;
        _isPlaying = true;
    }

    // Update is called once per frame

    void Update()
    {
        if (_isPlaying)
        {
            float phase = _localTime / _flashDuration;
            if (phase > 1)
            {
                _light.intensity = 0;        
                _isPlaying = false;
                _light.enabled = false;
            }
            
            _light.intensity = _flashCurve.Evaluate(phase) * _lightMaxIntensity;
            _localTime += Time.deltaTime;
        }
    }
}
