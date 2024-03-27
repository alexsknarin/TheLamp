using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour, IInitializable
{
    [SerializeField] private WaveText _waveText;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private AnimationCurve _cameraAnimationCurve;
    [SerializeField] private float _cameraAnimationDuration;
    [SerializeField] private Image _fadeImage;
    [SerializeField] private GameObject _analyticsConsentPanel;
    [SerializeField] private GameObject _analyticsConsentEnableButton;
    [SerializeField] private GameObject _analyticsConsentDisableButton;
    [SerializeField] private UGSSetup _ugsSetup;
    
    public event Action OnIntroFinished;
    
    private float _localTime;
    private bool _isIntroPlaying = false;
    
    private Color _fadeColor1 = new Color(0, 0, 0, 1);
    private Color _fadeColor2 = new Color(0, 0, 0, 0);
    private float _cameraStartZPosition = -6.5f;
    private float _cameraEndZPosition = -5.88f;
    
    public void Initialize()
    {
            _waveText.gameObject.SetActive(true);
        _waveText.HideWaveText();
        
        _fadeImage.gameObject.SetActive(true);
        _fadeImage.color = _fadeColor1;
        _isIntroPlaying = false;
        if (PlayerPrefs.HasKey("dataConsent"))
        {
            _analyticsConsentPanel.SetActive(false);
            if(PlayerPrefs.GetInt("dataConsent") == 1)
            {
                _analyticsConsentEnableButton.SetActive(false);
                _analyticsConsentDisableButton.SetActive(true);
            }
            else
            {
                _analyticsConsentEnableButton.SetActive(true);
                _analyticsConsentDisableButton.SetActive(false);
            }
        }
        else
        {
            _analyticsConsentPanel.SetActive(true);
            _analyticsConsentEnableButton.SetActive(false);
            _analyticsConsentDisableButton.SetActive(false);
        }
    }
    
    public void AllowDataCollection()
    {
        _ugsSetup.AllowDataCollection();
        _analyticsConsentPanel.SetActive(false);
        _analyticsConsentEnableButton.SetActive(false);
        _analyticsConsentDisableButton.SetActive(true);
    }
    
    public void RefuseDataCollection()
    {
        _ugsSetup.RefuseDataCollection();
        _analyticsConsentPanel.SetActive(false);
        _analyticsConsentEnableButton.SetActive(true);
        _analyticsConsentDisableButton.SetActive(false);
    }
    
    public void EnableDataCollection()
    {
        _ugsSetup.StartAnalyticsCollection();
        _analyticsConsentEnableButton.SetActive(false);
        _analyticsConsentDisableButton.SetActive(true);
    }
    
    public void DisableDataCollection()
    {
        _ugsSetup.StopAnalyticsCollection();
        _analyticsConsentEnableButton.SetActive(true);
        _analyticsConsentDisableButton.SetActive(false);
    }

    public void PlayIntro()
    {
        _localTime = 0;
        _isIntroPlaying = true;
        
    }
    
    public void StartPrepare(int wave)
    {
        _waveText.ShowWaveText(wave);
    }
    
    public void StartFight()
    {
        _waveText.HideWaveText();
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
