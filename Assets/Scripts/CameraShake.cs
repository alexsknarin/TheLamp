using System.Collections;
using UnityEngine;

/// <summary>
/// Component to perform a camera shake. Use Random.insideUnitCircle to get a random position each frame.
/// </summary>
public class CameraShake : MonoBehaviour
{
    [SerializeField] private float _shakeAmplitude;
    [SerializeField] private float _shakeDuration;
    [SerializeField] private AnimationCurve _shakeProfileCurve;
    
    private Vector3 _originalPos;
    private float _prevTime;
    private bool _isShaking;
    private bool _isPaused;
    private bool _isDamageable = true;
    private bool _isGameOver = false;
    
    private void Start()
    {
        _originalPos = transform.position;
        _isShaking = false;
    }
    
    private void Update()
    {
        if (!_isPaused || _isGameOver)
        {
            if (_isShaking)
            {
                PerformShake();
            }
        }
    }
    
    private void OnEnable()
    {
        Lamp.OnLampDamaged += StartShake;
    }

    private void OnDisable()
    {
        Lamp.OnLampDamaged -= StartShake;
    }
    
    private void StartShake()
    {
        _prevTime = Time.time;
        _isShaking = true;
        _isDamageable = false;
    }
    
    private void PerformShake()
    {
        if (Time.time - _prevTime < _shakeDuration)
        {
            float shakeMask = _shakeProfileCurve.Evaluate((Time.time - _prevTime) / _shakeDuration);
            transform.position = Vector3.Lerp(_originalPos, _originalPos + (Vector3)(Random.insideUnitCircle * _shakeAmplitude), shakeMask);            
        }
        else
        {
            _isShaking = false;
        }            
    }

}
