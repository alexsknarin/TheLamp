// TODO: remove Monobehaviour - provide this object via DI
// TODO: make dependable on UGS - to wait until connected to UGS
// TODO: Extract proper intefaces for both

using System;
using System.Collections;
using Unity.Services.Analytics;
using UnityEngine;

public class UnityAnalyticsService : MonoBehaviour, IAnalyticsService, IInitializable
{
    IGameSettingsService _gameSettingsService;
    private IUGSAuthenticationService _ugsAuthenticationService;
    public event Action OnConsentAddressedEvent;
    
    // Analytics Data
    private float _waveTime;
    private CustomEvent _waveEndEvent;
    private CustomEvent _lampDamageEvent;
    private CustomEvent _healthUpgradeEvent;
    private CustomEvent _coolUpgradeEvent;
    
    private bool _isEnabled;
    
    // Dependency Injection
    public void Construct(IGameSettingsService gameSettingsService, IUGSAuthenticationService ugsAuthenticationService)
    {
        _gameSettingsService = gameSettingsService;
        _ugsAuthenticationService = ugsAuthenticationService;
    }
    
    public void Initialize()
    {
        _waveEndEvent = new CustomEvent("waveFinished");
        _lampDamageEvent = new CustomEvent("LampDamaged");
        _healthUpgradeEvent = new CustomEvent("healthUpgrade");
        _coolUpgradeEvent = new CustomEvent("coolUpgrade");
        _gameSettingsService.OnIsDataCollectionEnabledChangedEvent += UpdateCollectionBehavior;
        
        Debug.Log("Analytics: Initializing Unity Analytics Service.");
        StartCoroutine(WaitUntilUGSConnected());
    }
    
    private IEnumerator WaitUntilUGSConnected() // TODO: check this later - need timeout and error handling
    {
        yield return new WaitUntil(() => _ugsAuthenticationService.IsConnected);
        CheckIfConsentIsProvided();
    }

    private void OnDestroy()
    {
        _gameSettingsService.OnIsDataCollectionEnabledChangedEvent -= UpdateCollectionBehavior;
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
            StartCoroutine(GetUserConsent());
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
        OnConsentAddressedEvent?.Invoke();  // TODO: needed for Game  class to know when to start the game - need to remove it from here
        AnalyticsService.Instance.StartDataCollection(); 
        Debug.Log("Analytics: Consent has been provided. The SDK is now collecting data");
    }

    private void StopAnalyticsCollection()
    {
        _isEnabled = false;
        OnConsentAddressedEvent?.Invoke(); // What is this used for?
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
}
