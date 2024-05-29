using System;
using UnityEngine;

public class UiIntroAnimation : MonoBehaviour
{
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private AnimationCurve _cameraAnimationCurve;
    private UnityEngine.Rendering.Universal.ColorAdjustments _colorAdjustments;
    private float _duration;

    private float _cameraStartZPosition = -6.5f;
    private float _cameraEndZPosition = -5.88f;
    private float _localTime;
    private bool _isPlaying;

    public event Action OnIntroFinished;

    // Update is called once per frame
    
    public void Play(float duration, UnityEngine.Rendering.Universal.ColorAdjustments colorAdjustments)
    {
        _duration = duration;
        _localTime = 0;
        _isPlaying = true;
        _colorAdjustments = colorAdjustments;
    }
    
    void Update()
    {
        if (_isPlaying)
        {
            float phase = _localTime / _duration;
            Vector3 cameraPosition = _cameraTransform.position;
            if (phase > 1)
            {
                _isPlaying = false;
                _localTime = 0;
                cameraPosition.z = _cameraEndZPosition;
                _cameraTransform.position = cameraPosition;
                _colorAdjustments.postExposure.Override(0);
                OnIntroFinished?.Invoke();
            }
            _colorAdjustments.postExposure.Override(Mathf.Lerp(-8, 0, phase));
            cameraPosition.z = Mathf.Lerp(_cameraStartZPosition, _cameraEndZPosition, _cameraAnimationCurve.Evaluate(phase));
            _cameraTransform.position = cameraPosition;
            _localTime += Time.deltaTime;
        }
    }
}
