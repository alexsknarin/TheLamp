using System;
using System.Collections;
using Unity.Services.Analytics;
using UnityEngine;

public class UnityAnalyticsService : MonoBehaviour, IAnalyticsService // TODO: Remove monobehaviour - use DI container
{
    // TODO: Extract User Consent provider ??? 
    // TODO: Invert Dependencies - it should provide service to other classes
    // TODO: test if it works properly - maybe still need to wait until connected??? - check this 
    
    [SerializeField] private LampStatsManager _lampStatsManager; // Inject
    [SerializeField] private UiManager _sceneUiManager; // Inject
    public event Action OnConsentAddressedEvent;

    private bool _isConsentSet = false;
    private bool _isConsentGiven = false;

    // Analytics Data
    private float _waveTime;
    private CustomEvent _waveEndEvent;
    private CustomEvent _lampDamageEvent;
    private CustomEvent _healthUpgradeEvent;
    private CustomEvent _coolUpgradeEvent;

    private void OnEnable()
    {
        EnemyManager.OnWaveStartedEvent += HandleWaveStart; // Inject
        EnemyManager.OnWaveEndedEvent += SubmitWaveEndEvent; // Inject
        Lamp.OnLampDamagedEvent += SubmitLampDamageEvent; // Inject
        _lampStatsManager.OnHealthChangeEvent += SubmitHealthUpgradeEvent; // Inject
        _lampStatsManager.OnCooldownUpgradedEvent += SubmitCoolUpgradeEvent; // Inject
    }

    private void OnDisable()
    {
        EnemyManager.OnWaveStartedEvent -= HandleWaveStart;
        EnemyManager.OnWaveEndedEvent -= SubmitWaveEndEvent;
        Lamp.OnLampDamagedEvent -= SubmitLampDamageEvent;
        _lampStatsManager.OnHealthChangeEvent -= SubmitHealthUpgradeEvent;
        _lampStatsManager.OnCooldownUpgradedEvent -= SubmitCoolUpgradeEvent;
    }

    public void Initialize()
    {
        _waveEndEvent = new CustomEvent("waveFinished");
        _lampDamageEvent = new CustomEvent("LampDamaged");
        _healthUpgradeEvent = new CustomEvent("healthUpgrade");
        _coolUpgradeEvent = new CustomEvent("coolUpgrade");
        
        Debug.Log("Analytics: Initializing Unity Analytics Service.");
        CheckIfConsentIsProvided();
    }

    public void SetConsentData(bool isConsentGiven)
    {
        _isConsentSet = true;
        _isConsentGiven = isConsentGiven;
    }

    public void UpdateCollectionBehavior(bool isConsentGiven)
    {
        if (isConsentGiven)
        {
            StartAnalyticsCollection();
        }
        else
        {
            StopAnalyticsCollection();
        }
    }

    private void CheckIfConsentIsProvided()
    {
        if (PlayerPrefs.GetInt("dataConsentSet") == 1 && PlayerPrefs.GetInt("dataConsent") == 1) // Consent yes 
        {
            StartAnalyticsCollection();
        }
        else if (PlayerPrefs.GetInt("dataConsentSet") == 0) // Consent not set
        {
            Debug.Log("Analytics: Consent data doesn't exist. Awaiting User Input");
            StartCoroutine(GetUserConsent());
        }
        else
        {
            Debug.Log("Analytics: Consent has not been provided. The SDK is not collecting data");
        }
    }

    private IEnumerator GetUserConsent()
    {
        yield return new WaitUntil(() => _isConsentSet);
        if (_isConsentGiven)
        {
            StartAnalyticsCollection();
        }
        else
        {
            StopAnalyticsCollection();
        }
    }

    public void StartAnalyticsCollection()
    {
        PlayerPrefs.SetInt("dataConsentSet", 1);
        PlayerPrefs.SetInt("dataConsent", 1);
        PlayerPrefs.Save();
        _isConsentGiven = true;
        OnConsentAddressedEvent?.Invoke();  // What is this used for?
        
        AnalyticsService.Instance.StartDataCollection(); 
        Debug.Log("Analytics: Consent has been provided. The SDK is now collecting data");
    }

    public void StopAnalyticsCollection()
    {
        PlayerPrefs.SetInt("dataConsentSet", 1);
        PlayerPrefs.SetInt("dataConsent", 0);
        PlayerPrefs.Save();
        _isConsentGiven = false;
        OnConsentAddressedEvent?.Invoke(); // What is this used for?
        AnalyticsService.Instance.StopDataCollection();
        Debug.Log("Analytics: Consent has been refused. The SDK is not collecting data");
    }

    private void HandleWaveStart(int wave)
    {
        _waveTime = Time.time;
    }

    private void SubmitWaveEndEvent(int wave)
    {
        _waveTime = Time.time - _waveTime;
        if (_isConsentGiven)
        {
            _waveEndEvent.Reset();
            _waveEndEvent.Add("waveNum", wave);
            _waveEndEvent.Add("waveTime", _waveTime);
            AnalyticsService.Instance.RecordEvent(_waveEndEvent);
            
        }
    }

    private void SubmitLampDamageEvent(EnemyBase enemy)
    {
        if (_isConsentGiven)
        {
            _lampDamageEvent.Reset();
            _lampDamageEvent.Add("enemyType", enemy.EnemyType.ToString());
            AnalyticsService.Instance.RecordEvent(_lampDamageEvent);
        }
    }

    private void SubmitHealthUpgradeEvent()
    {
        if (_isConsentGiven)
        {
            _healthUpgradeEvent.Reset();
            AnalyticsService.Instance.RecordEvent(_healthUpgradeEvent);
        }
    }

    private void SubmitCoolUpgradeEvent()
    {
        if (_isConsentGiven)
        {
            _coolUpgradeEvent.Reset();
            AnalyticsService.Instance.RecordEvent(_coolUpgradeEvent);
        }
    }
}
