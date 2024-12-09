using System;
using UnityEngine;
using UnityEngine.Rendering;

public class UiManager : MonoBehaviour, IInitializable
{
    // TODO: Switch button presses to AddListener
    [SerializeField] private UiText _waveText;
    [SerializeField] private Transform _cameraTransform; // TODO: Remove this - not used
    [SerializeField] private AnimationCurve _cameraAnimationCurve;

    [Header("Upgrade Panel")] [SerializeField]
    private LampStatsManager _lampStatsManager;

    [SerializeField] private GameObject _upgradeButtonsPanel;
    [SerializeField] private UiUpgradePoints _uiUpgradePoints;
    [SerializeField] private GameObject _upgradeHintsPanel;
    [SerializeField] private UpgradeButton _upgradeHealthButton;
    [SerializeField] private UpgradeButton _upgradeCooldownButton;
    [SerializeField] private UpgradeButton _upgradeAttackDistanceButton;

    [Header("Overlay Images")] [SerializeField]
    private BrokenGlassEffect _brokenGlassEffect;

    [Header("Animation:")] [Header("Intro")] [SerializeField]
    private UiIntroAnimation _uiIntroAnimation;

    [SerializeField] private float _introDuration;
    [Header("Prepare")] [SerializeField] private UiStartPrepareAnimation _uiStartPrepareAnimation;
    [Header("Fight")] [SerializeField] private UiStartFightAnimation _uiStartFightAnimation;
    [Header("Game Over")] [SerializeField] private UiGameOverAnimation _uiGameOverAnimation;
    [SerializeField] private float _gameOverDuration;
    [SerializeField] private GameObject _gameOverPanel;
    [SerializeField] private UiText _gameOverText;
    [SerializeField] private GameObject _gameOverButtonsGroup;
    [SerializeField] private Volume _postProcessingVolume;

    // Dependencies
    // private IAnalyticsService _analyticsService;
    
    private UnityEngine.Rendering.Universal.ColorAdjustments _colorAdjustments;
    public event Action OnIntroFinishedEvent;
    public event Action OnGameoverFinishedEvent;
    public event Action<bool> OnDataConsentSetEvent;
    public event Action<bool> OnAnalyticsCollectionChangeEvent;

    private float _localTime;

    // TODO: find a way to have less events
    private void OnEnable()
    {
        Lamp.OnLampDamagedEvent += HandleLampDamage;
        Lamp.OnLampDeadEvent += HandleLampDeath;
        _uiIntroAnimation.OnIntroFinishedEvent += OnIntroFinishedHandler;
        _uiGameOverAnimation.OnGameOverAnimationFinishedEvent += HandleGameoverAnimationFinished;
    }

    private void OnDisable()
    {
        Lamp.OnLampDamagedEvent -= HandleLampDamage;
        Lamp.OnLampDeadEvent -= HandleLampDeath;
        _uiIntroAnimation.OnIntroFinishedEvent -= OnIntroFinishedHandler;
        _uiGameOverAnimation.OnGameOverAnimationFinishedEvent -= HandleGameoverAnimationFinished;
    }
    
    public void Initialize()
    {
        VolumeProfile volumeProfile = _postProcessingVolume.profile;
        if (!volumeProfile) throw new System.NullReferenceException(nameof(UnityEngine.Rendering.VolumeProfile));
        // You can leave this variable out of your function, so you can reuse it throughout your class.
        if (!volumeProfile.TryGet(out _colorAdjustments))
            throw new System.NullReferenceException(nameof(_colorAdjustments));
        _colorAdjustments.postExposure.Override(-15);

        _waveText.gameObject.SetActive(true);
        _waveText.DisableText();
        _upgradeButtonsPanel.SetActive(false);
        _gameOverPanel.SetActive(false);
        _gameOverButtonsGroup.SetActive(false);
    }

    public void SetIntroDuration(float duration)
    {
        _introDuration = duration;
    }

    public void PlayIntro()
    {
        _uiIntroAnimation.Play(_introDuration, _colorAdjustments);
    }

    private void OnIntroFinishedHandler()
    {
        OnIntroFinishedEvent?.Invoke();
    }

    public void StartPrepare(int wave)
    {
        _uiStartPrepareAnimation.Play(1f, wave, _lampStatsManager);
        RefreshAllUpgradeButtons();
    }

    public void StartFight()
    {
        _uiStartFightAnimation.Play(1f);
    }

    public void HandleUpgradeButtonClick(int upgradeType)
    {
        if (upgradeType == 0)
        {
            // Upgrade Health
            _lampStatsManager.UpgradeHealth();
            RefreshAllUpgradeButtons();
        }
        else if (upgradeType == 1)
        {
            // Upgrade Cooldown
            _lampStatsManager.UpgradeCooldown();
            RefreshAllUpgradeButtons();
        }
        else if (upgradeType == 2)
        {
            // Upgrade Attack Distance
            _lampStatsManager.UpgradeAttackDistance();
            RefreshAllUpgradeButtons();
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
        _brokenGlassEffect.Play(BrokenGlassEventType.Damage);
    }

    private void HandleLampDeath(EnemyBase enemy)
    {
        _brokenGlassEffect.Play(BrokenGlassEventType.Death);
    }

    private void HandleGameoverAnimationFinished()
    {
        _gameOverButtonsGroup.SetActive(true);
        OnGameoverFinishedEvent?.Invoke();
    }
    
    private void RefreshAllUpgradeButtons()
    {
        RefreshUpgradeButton(_lampStatsManager.HealthUpgradeStatus(), _upgradeHealthButton);
        RefreshUpgradeButton(_lampStatsManager.CooldownUpgradeStatus(), _upgradeCooldownButton);
        RefreshUpgradeButton(_lampStatsManager.AttackDistanceUpgradeStatus(), _upgradeAttackDistanceButton);
    }

    private void RefreshUpgradeButton(UpgradeStatus status, UpgradeButton button)
    {
        switch (status)
        {
            case UpgradeStatus.MaxedOut:
                button.DisableButton();
                break;
            case UpgradeStatus.ReadyForUpgrade:
                button.EnableButton();
                break;
            case UpgradeStatus.NotEnoughPoints:
                button.DisableButton();
                break;
        }
    }
}
