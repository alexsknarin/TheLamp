using System;
using System.Collections;
using Unity.Services.Analytics;
using UnityEngine;

public class UnityAnalyticsService : IAnalyticsService, IInitializable, IDisposable
{
    private IGameSettingsService _gameSettingsService;
    private IUGSAuthenticationService _ugsAuthenticationService;
    private CoroutineHost _coroutineHost;
    private IGameConfigService _gameConfigService;
    
    // Analytics Data
    private float _waveTime;
    private CustomEvent _waveEndEvent;
    private CustomEvent _lampDamageEvent;
    private CustomEvent _healthUpgradeEvent;
    private CustomEvent _coolUpgradeEvent;
    private CustomEvent _attackUpgradeEvent;

    private bool _isEnabled;
    private float _localTime;

    public UnityAnalyticsService(
        IGameSettingsService gameSettingsService,
        IUGSAuthenticationService ugsAuthenticationService,
        CoroutineHost coroutineHost,
        IGameConfigService gameConfigService
    )
    {
        _gameSettingsService = gameSettingsService;
        _ugsAuthenticationService = ugsAuthenticationService;
        _coroutineHost = coroutineHost;
        _gameConfigService = gameConfigService;
        
        _gameSettingsService.IsDataCollectionEnabledChanged += UpdateCollectionBehavior;
    }

    public void Dispose()
    {
        _gameSettingsService.IsDataCollectionEnabledChanged -= UpdateCollectionBehavior;
    }
    
    public event Action ConsentAddressed;

    // Dependency Injection
    public void Initialize()
    {
        _waveEndEvent = new CustomEvent("waveFinished");
        _lampDamageEvent = new CustomEvent("LampDamaged");
        _healthUpgradeEvent = new CustomEvent("healthUpgrade");
        _coolUpgradeEvent = new CustomEvent("coolUpgrade");
        _attackUpgradeEvent = new CustomEvent("attackUpgrade");
        Debug.Log("Analytics: Initializing Unity Analytics Service.");
        _localTime = 0;
        _coroutineHost.StartCoroutine(WaitUntilUgsConnected());
    }

    // Analytics Event Calls
    public void SubmitWaveStartEvent(int wave)
    {
        _waveTime = Time.time;
    }

    public void SubmitWaveEndEvent(int wave)
    {
        _waveTime = Time.time - _waveTime;
        if (_isEnabled)
        {
            _waveEndEvent.Reset();
            _waveEndEvent.Add("waveNum", wave);
            _waveEndEvent.Add("waveTime", _waveTime);
            AnalyticsService.Instance.RecordEvent(_waveEndEvent);
        }
    }

    public void SubmitLampDamageEvent(EnemyBase enemy)
    {
        if (_isEnabled)
        {
            _lampDamageEvent.Reset();
            _lampDamageEvent.Add("enemyType", enemy.EnemyType.ToString());
            AnalyticsService.Instance.RecordEvent(_lampDamageEvent);
        }
    }

    public void SubmitHealthUpgradeEvent()
    {
        if (_isEnabled)
        {
            _healthUpgradeEvent.Reset();
            AnalyticsService.Instance.RecordEvent(_healthUpgradeEvent);
        }
    }

    public void SubmitCoolUpgradeEvent()
    {
        if (_isEnabled)
        {
            _coolUpgradeEvent.Reset();
            AnalyticsService.Instance.RecordEvent(_coolUpgradeEvent);
        }
    }
    
    public void SubmitAttackUpgradeEvent()
    {
        if (_isEnabled)
        {
            _attackUpgradeEvent.Reset();
            AnalyticsService.Instance.RecordEvent(_attackUpgradeEvent);
        }
    }

    private IEnumerator WaitUntilUgsConnected()
    {
        while (_localTime < _gameConfigService.GameConfig.AnalyticsTimeOutTime)
        {
            if (_ugsAuthenticationService.IsConnected)
            {
                CheckIfConsentIsProvided();
                yield break;
            }
            _localTime += Time.deltaTime;
            yield return null;
        }
        Debug.Log("Analytics: UGS Connection Timed Out. Skipping Analytics");
    }

    private void CheckIfConsentIsProvided()
    {
        if (_gameSettingsService.IsConsentSet && _gameSettingsService.IsDataCollectionEnabled)
        {
            StartAnalyticsCollection();
        }
        else if (_gameSettingsService.IsConsentSet && !_gameSettingsService.IsDataCollectionEnabled)
        {
            StopAnalyticsCollection();
        }
        else
        {
            Debug.Log("Analytics: Consent data doesn't exist. Awaiting User Input");
            _coroutineHost.StartCoroutine(GetUserConsent());
        }
    }

    private IEnumerator GetUserConsent()
    {
        yield return new WaitUntil(() => _gameSettingsService.IsConsentSet);
        if (_isEnabled)
        {
            StartAnalyticsCollection();
        }
        else
        {
            StopAnalyticsCollection();
        }
    }

    private void StartAnalyticsCollection()
    {
        _isEnabled = true;
        ConsentAddressed?.Invoke();  // TODO: needed for Game  class to know when to start the game - need to remove it from here
        AnalyticsService.Instance.StartDataCollection(); 
        Debug.Log("Analytics: Consent has been provided. The SDK is now collecting data");
    }

    private void StopAnalyticsCollection()
    {
        _isEnabled = false;
        ConsentAddressed?.Invoke(); // What is this used for?
        AnalyticsService.Instance.StopDataCollection();
        Debug.Log("Analytics: Consent has been refused. The SDK is not collecting data");
    }

    private void UpdateCollectionBehavior(bool isConsentGiven)
    {
        if (isConsentGiven)
        {
            StartAnalyticsCollection();
            Debug.Log("Disable Enabled");
        }
        else
        {
            StopAnalyticsCollection();
            Debug.Log("Analytics Disabled");
        }
    }
}
