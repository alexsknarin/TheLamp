using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UiManager : MonoBehaviour, IInitializable
{
    [SerializeField] private UiText _waveText;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private AnimationCurve _cameraAnimationCurve;
    [Header("Upgrade Panel")]
    [SerializeField] private LampStatsManager _lampStatsManager;
    [SerializeField] private GameObject _upgradeButtonsPanel;
    [SerializeField] private UiUpgradePoints _uiUpgradePoints;
    [SerializeField] private GameObject _upgradeHintsPanel;
    [Header("Overlay Images")]
    [SerializeField] private BrokenGlassEffect _brokenGlassEffect;
    [Header("Analytics")]
    [SerializeField] private GameObject _analyticsConsentPanel;
    [SerializeField] private GameObject _analyticsConsentEnableButton;
    [SerializeField] private GameObject _analyticsConsentDisableButton;
    [SerializeField] private UGSSetup _ugsSetup;
    [Header("Animation:")]
    [Header("Intro")]
    [SerializeField] private UiIntroAnimation _uiIntroAnimation;
    [SerializeField] private float _introDuration;
    [Header("Prepare")]
    [SerializeField] private UiStartPrepareAnimation _uiStartPrepareAnimation;
    [Header("Fight")]
    [SerializeField] private UiStartFightAnimation _uiStartFightAnimation;
    [Header("Game Over")]
    [SerializeField] private UiGameOverAnimation _uiGameOverAnimation;
    [SerializeField] private float _gameOverDuration;
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private UiText _gameOverText;
    [SerializeField] private GameObject _gameOverButtonsGroup;
    [SerializeField] private Volume _postProcessingVolume;
    private UnityEngine.Rendering.Universal.ColorAdjustments _colorAdjustments;
    
    public event Action OnIntroFinished;
    
    private float _localTime;
    private bool _isGameOverPlaying = false;
    
    // TODO: find a way to have less events
    private void OnEnable()
    {
        Lamp.OnLampDamaged += HandleLampDamage;
        Lamp.OnLampDead += HandleLampDeath;
        _uiIntroAnimation.OnOnFinished += OnIntroFinishedHandler;
    }
    
    private void OnDisable()
    {
        Lamp.OnLampDamaged -= HandleLampDamage;
        Lamp.OnLampDead -= HandleLampDeath;
        _uiIntroAnimation.OnOnFinished -= OnIntroFinishedHandler;
    }
    
    
    public void Initialize()
    {
        VolumeProfile volumeProfile = _postProcessingVolume.profile;
        if(!volumeProfile) throw new System.NullReferenceException(nameof(UnityEngine.Rendering.VolumeProfile));
        // You can leave this variable out of your function, so you can reuse it throughout your class.
        if(!volumeProfile.TryGet(out _colorAdjustments)) throw new System.NullReferenceException(nameof(_colorAdjustments));
        _colorAdjustments.postExposure.Override(-15);

        _waveText.gameObject.SetActive(true);
        _waveText.DisableText();
        _upgradeButtonsPanel.SetActive(false);
        _gameOverPanel.SetActive(false);
        _gameOverButtonsGroup.SetActive(false);

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
    
    public void SetIntroDuration(float duration)
    {
        _introDuration = duration;
    }
    
    public void HandleYesDataCollectionBtn()
    {
        _ugsSetup.AllowDataCollection();
        _analyticsConsentPanel.SetActive(false);
        _analyticsConsentEnableButton.SetActive(false);
        _analyticsConsentDisableButton.SetActive(true);
    }
    
    public void HandleNoDataCollectionBtn()
    {
        _ugsSetup.RefuseDataCollection();
        _analyticsConsentPanel.SetActive(false);
        _analyticsConsentEnableButton.SetActive(true);
        _analyticsConsentDisableButton.SetActive(false);
    }
    
    public void HandleEnableDataCollectionBtn()
    {
        _ugsSetup.StartAnalyticsCollection();
        _analyticsConsentEnableButton.SetActive(false);
        _analyticsConsentDisableButton.SetActive(true);
    }
    
    public void HandleDisableDataCollectionBtn()
    {
        _ugsSetup.StopAnalyticsCollection();
        _analyticsConsentEnableButton.SetActive(true);
        _analyticsConsentDisableButton.SetActive(false);
    }

    public void PlayIntro()
    {
        _uiIntroAnimation.Play(_introDuration, _colorAdjustments);
    }

    private void OnIntroFinishedHandler()
    {
        OnIntroFinished?.Invoke();
    }
    
    public void StartPrepare(int wave)
    {
        _uiStartPrepareAnimation.Play(1f, wave, _lampStatsManager);
    }
    
    public void StartFight()
    {
        _uiStartFightAnimation.Play(1f);
    }
    
    public void HandleUpgradeButtonClick(int upgradeType)
    {
        if(upgradeType == 0)
        {
            // Upgrade Health
            _lampStatsManager.UpgradeHealth();
        }
        else if (upgradeType == 1)
        {
            // Upgrade Cooldown
            _lampStatsManager.UpgradeCooldown();
        }
        
        if (_lampStatsManager.UpgradePoints == 0)
        {
            _upgradeButtonsPanel.SetActive(false);
        }
        else
        {
            _uiUpgradePoints.ShowUpgradePoints(_lampStatsManager.UpgradePoints);
        }
    }

    public void StartGameOver()
    {
        _uiGameOverAnimation.Play(_gameOverDuration, _colorAdjustments);
    }

    private void HandleLampDamage(EnemyBase enemy)
    {
        _brokenGlassEffect.Play(BrokenGlassEventTypes.Damage);
    }
    
    private void HandleLampDeath(EnemyBase enemy)
    {
        _brokenGlassEffect.Play(BrokenGlassEventTypes.Death);
    }
    
    public void ShowUpgradeButtons()
    {
        _upgradeButtonsPanel.SetActive(true);
    }
}
