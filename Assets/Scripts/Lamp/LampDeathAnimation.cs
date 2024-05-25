using UnityEngine;

public class LampDeathAnimation : MonoBehaviour
{
    [SerializeField] private AnimationCurve _animCurve;
    [SerializeField] private AnimationCurve _damageAnimCurve;
    [SerializeField] private LampHealthBar _lampHealthBar;
    [SerializeField] private LampEmissionController _lampEmissionController;

    private bool _isPlaying = false;
    private float _duration;
    private float _localTime = 0;
    
    public void Play(float duration)
    {
        _lampEmissionController.Intensity = 1;
        _lampEmissionController.IsDamageEnabled = true;
        _lampEmissionController.DamageMix = 1;
        
        _duration = duration;
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
                
                // NEW LAMP
                _lampEmissionController.Intensity = 0;
                _lampEmissionController.DamageMix = 0;
                _lampEmissionController.IsDamageEnabled = false;
                
                _lampHealthBar.UpdateHealth(0, 0);
                return;
            }
        
            float phaseAnimated = _animCurve.Evaluate(phase);
           
            // NEW LAMP
            _lampEmissionController.Intensity = phaseAnimated;
            _lampEmissionController.DamageMix = _damageAnimCurve.Evaluate(phase);
        
            _localTime += Time.deltaTime;
        }
    }
}
