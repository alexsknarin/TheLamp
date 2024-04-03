using System;
using UnityEngine;
using UnityEngine.UI;

public class UiIntroAnimation : MonoBehaviour
{
    [SerializeField] private Image _fadeImage;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private AnimationCurve _cameraAnimationCurve;
    private float _duration;

    private float _cameraStartZPosition = -6.5f;
    private float _cameraEndZPosition = -5.88f;
    private float _localTime;
    private bool _isPlaying;

    private Color _fadeColor1 = new Color(0, 0, 0, 1);
    private Color _fadeColor2 = new Color(0, 0, 0, 0);

    public event Action OnOnFinished;

    // Update is called once per frame
    
    public void Play(float duration)
    {
        _duration = duration;
        _localTime = 0;
        _isPlaying = true;
        _fadeImage.gameObject.SetActive(true);
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
                _fadeImage.color = _fadeColor1;
                cameraPosition.z = _cameraEndZPosition;
                _cameraTransform.position = cameraPosition;
                _fadeImage.gameObject.SetActive(false);
                OnOnFinished?.Invoke();
            }
            _fadeImage.color = Color.Lerp(_fadeColor1, _fadeColor2, phase);
            cameraPosition.z = Mathf.Lerp(_cameraStartZPosition, _cameraEndZPosition, _cameraAnimationCurve.Evaluate(phase));
            _cameraTransform.position = cameraPosition;
            _localTime += Time.deltaTime;
        }
    }
}
