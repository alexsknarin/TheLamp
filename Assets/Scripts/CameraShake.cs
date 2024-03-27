using UnityEngine;

/// <summary>
/// Component to perform a camera shake. Use Random.insideUnitCircle to get a random position each frame.
/// </summary>
public class CameraShake : MonoBehaviour
{
    [SerializeField] private float _shakeAmplitude;
    [SerializeField] private float _shakeDuration;
    [SerializeField] private AnimationCurve _shakeProfileCurve;
    [Header("Enemy proximity shake settings")]
    [SerializeField] private float _waspProximityShakeAmplitude;
    [SerializeField] private AnimationCurve _waspProximityCurve;
    [SerializeField] private Transform _waspTransform;
    [SerializeField] private float _waspProximityMaxDistance;
    [SerializeField] private float _waspProximityMinDistance;
    private float _shakeDistance;
    private bool _isWaspNearby;

    private Vector3 _originalPos;
    private float _localTime;
    private bool _isShaking;
    private bool _isPaused;
    private bool _isGameOver = false;
    private float _shakeAmplitudeMultiplier = 1.0f;
    
    private void Start()
    {
        _originalPos = transform.position;
        _shakeDistance = Mathf.Abs(_waspProximityMaxDistance - _waspProximityMinDistance);
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
        
        if (_waspTransform.position.z < _waspProximityMaxDistance)
        {
            float shakephase = Mathf.Abs(_waspTransform.position.z - _waspProximityMaxDistance) / _shakeDistance;
            transform.position = Vector3.Lerp(_originalPos, _originalPos + (Vector3)(Random.insideUnitCircle * _waspProximityShakeAmplitude), shakephase);   
        }
    }
    
    private void OnEnable()
    {
        Lamp.OnLampDamaged += StartDamageShake;
        EnemyManager.OnFireflyExplosion += StartExplosionShake;
    }

    private void OnDisable()
    {
        Lamp.OnLampDamaged -= StartDamageShake;
        EnemyManager.OnFireflyExplosion -= StartExplosionShake;
    }
    
    private void StartDamageShake(EnemyBase enemy)
    {
        _localTime = 0;
        _isShaking = true;
        _shakeAmplitudeMultiplier = 1.0f;
    }
    
    private void StartExplosionShake()
    {
        _localTime = 0;
        _isShaking = true;
        _shakeAmplitudeMultiplier = 2.5f;
    }
    
    private void PerformShake()
    {
        if (_localTime < _shakeDuration)
        {
            float shakeMask = _shakeProfileCurve.Evaluate(_localTime / _shakeDuration) * _shakeAmplitudeMultiplier;
            transform.position = Vector3.Lerp(_originalPos, _originalPos + (Vector3)(Random.insideUnitCircle * _shakeAmplitude), shakeMask);            
        }
        else
        {
            _isShaking = false;
        }
        _localTime += Time.deltaTime;
    }
}
