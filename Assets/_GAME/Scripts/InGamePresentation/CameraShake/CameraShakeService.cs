using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.InGamePresentation.CameraShake
{
    public class CameraShakeService : MonoBehaviour, IInitializable
    {
        [Header("Shake Settings")]
        [SerializeField] private float _shakeAmplitude;
        [SerializeField] private float _shakeDuration;
        [SerializeField] private AnimationCurve _shakeProfileCurve;
        private IStrategy _cameraShakeStrategy;
        private bool _isStrategyEnabled;
        private Vector3 _originalPos;
        private Vector3 _displaceVector;
        private float _localTime;
        private bool _isShaking;
        private bool _isGameOver = false;
        private float _shakeAmplitudeMultiplier = 1.0f;

    
        public void Initialize()
        {
            _originalPos = transform.localPosition;
            _isStrategyEnabled = false;
            _isShaking = false;
        }
    
        public void StartDamageShake()
        {
            _localTime = 0;
            _isShaking = true;
            _shakeAmplitudeMultiplier = 1.0f;
        }
    
        public void StartExplosionShake()
        {
            _localTime = 0;
            _isShaking = true;
            _shakeAmplitudeMultiplier = 2.5f;
        }

        public void EnableBossShake(IStrategy shakeStrategy)
        {
            _cameraShakeStrategy = shakeStrategy;
            _isStrategyEnabled = true;
        }
    
        public void DisableBossShake()
        {
            _cameraShakeStrategy = null;
            _isStrategyEnabled = false;
        }
    
        private Vector3 PerformShake()
        {
            if (_localTime < _shakeDuration)
            {
                float shakeMask = _shakeProfileCurve.Evaluate(_localTime / _shakeDuration) * _shakeAmplitudeMultiplier;
                _localTime += Time.deltaTime;
                return Vector3.Lerp(Vector3.zero, (Vector3)(Random.insideUnitCircle * _shakeAmplitude), shakeMask);
            }
            _isShaking = false;
            return Vector3.zero;
        }
        private void Update()
        {
            if (_isShaking && !_isGameOver)
            {
                transform.localPosition = _originalPos + PerformShake();
            }
        
            if (_isStrategyEnabled)
            {
                if (_cameraShakeStrategy == null)
                    return;
                _displaceVector = _cameraShakeStrategy.Execute();
                transform.localPosition = _originalPos + _displaceVector;
            }
        }
    }
}
