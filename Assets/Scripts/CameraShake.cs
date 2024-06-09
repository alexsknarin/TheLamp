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
    [Header("Megamothling")]
    [SerializeField] private float _megamothlingProximityShakeAmplitude;
    [SerializeField] private AnimationCurve _megamothlingProximityCurve;
    [SerializeField] private Transform _megamothlingTransform;
    [SerializeField] private float _megamothlingProximityMaxDistance;
    [SerializeField] private float _megamothlingProximityMinDistance;
    
    [Header("Wasp")]
    [SerializeField] private float _waspProximityShakeAmplitude;
    [SerializeField] private AnimationCurve _waspProximityCurve;
    [SerializeField] private Transform _waspTransform;
    [SerializeField] private float _waspProximityMaxDistance;
    [SerializeField] private float _waspProximityMinDistance;
    
    [Header("Megabeetle")]
    [SerializeField] private float _megabeetleProximityShakeAmplitude;
    [SerializeField] private AnimationCurve _megabeetleProximityCurve;
    [SerializeField] private Transform _megabeetleTransform;
    [SerializeField] private float _megabeetleProximityMaxDistance;
    [SerializeField] private float _megabeetleProximityMinDistance;
    
    private float _waspShakeDistance;
    private float _megamothlingShakeDistance;
    private float _megabeetleShakeDistance;
    private bool _isWaspNearby;

    private Vector3 _originalPos;
    private float _localTime;
    private bool _isShaking;
    private bool _isPaused;
    private bool _isGameOver = false;
    private float _shakeAmplitudeMultiplier = 1.0f;
    
    private void OnEnable()
    {
        Lamp.OnLampDamaged += StartDamageShake;
        Lamp.OnLampDead += StartDamageShake;
        EnemyManager.OnFireflyExplosion += StartExplosionShake;
    }

    private void OnDisable()
    {
        Lamp.OnLampDamaged -= StartDamageShake;
        Lamp.OnLampDead -= StartDamageShake;
        EnemyManager.OnFireflyExplosion -= StartExplosionShake;
    }
    
    private void Start()
    {
        _originalPos = transform.position;
        _waspShakeDistance = Mathf.Abs(_waspProximityMaxDistance - _waspProximityMinDistance);
        _megamothlingShakeDistance = Mathf.Abs(_megamothlingProximityMaxDistance - _megamothlingProximityMinDistance);
        _megabeetleShakeDistance = Mathf.Abs(_megabeetleProximityMaxDistance - _megabeetleProximityMinDistance);
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
            float shakephase = Mathf.Abs(_waspTransform.position.z - _waspProximityMaxDistance) / _waspShakeDistance;
            transform.position = Vector3.Lerp(_originalPos, _originalPos + (Vector3)(Random.insideUnitCircle * _waspProximityShakeAmplitude), shakephase);   
        }
        
        if (_megamothlingTransform.position.z < _megamothlingProximityMaxDistance)
        {
            float shakephase = Mathf.Abs(_megamothlingTransform.position.z - _megamothlingProximityMaxDistance) / _megamothlingShakeDistance;
            transform.position = Vector3.Lerp(_originalPos, _originalPos + (Vector3)(Random.insideUnitCircle * _megamothlingProximityShakeAmplitude), shakephase);   
        }
        
        if (_megabeetleTransform.position.z < _megabeetleProximityMaxDistance)
        {
            float shakephase = Mathf.Abs(_megabeetleTransform.position.z - _megabeetleProximityMaxDistance) / _megabeetleShakeDistance;
            transform.position = Vector3.Lerp(_originalPos, _originalPos + (Vector3)(Random.insideUnitCircle * _megabeetleProximityShakeAmplitude), shakephase);   
        }
        
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
