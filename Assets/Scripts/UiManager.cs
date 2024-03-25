using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour, IInitializable
{
    [SerializeField] private GameObject _waveStartText;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private AnimationCurve _cameraAnimationCurve;
    [SerializeField] private float _cameraAnimationDuration;
    [SerializeField] private Image _fadeImage;
    
    public event Action OnIntroFinished;
    
    private float _localTime;
    private bool _isIntroPlaying = false;
    
    private Color _fadeColor1 = new Color(0, 0, 0, 1);
    private Color _fadeColor2 = new Color(0, 0, 0, 0);
    private float _cameraStartZPosition = -6.5f;
    private float _cameraEndZPosition = -5.88f;
    
    public void Initialize()
    {
        _waveStartText.gameObject.SetActive(false);
        _fadeImage.gameObject.SetActive(true);
        _fadeImage.color = _fadeColor1;
        _isIntroPlaying = false;
    }

    public void PlayIntro()
    {
        _localTime = 0;
        _isIntroPlaying = true;
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_isIntroPlaying)
        {
            float phase = _localTime / _cameraAnimationDuration;
            Vector3 cameraPosition = _cameraTransform.position;
            if (phase > 1)
            {
                _isIntroPlaying = false;
                _localTime = 0;
                _fadeImage.color = _fadeColor1;
                cameraPosition.z = _cameraEndZPosition;
                _cameraTransform.position = cameraPosition;
                _waveStartText.SetActive(true);
                _fadeImage.gameObject.SetActive(false);
                OnIntroFinished?.Invoke();
            }
            _fadeImage.color = Color.Lerp(_fadeColor1, _fadeColor2, phase);
            cameraPosition.z = Mathf.Lerp(_cameraStartZPosition, _cameraEndZPosition, _cameraAnimationCurve.Evaluate(phase));
            _cameraTransform.position = cameraPosition;
            
            _localTime += Time.deltaTime;
        }
    }

    
}
