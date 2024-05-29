using System.Collections.Generic;
using Unity.Services.Analytics;
using UnityEngine;

public class AnalyticsCollector : MonoBehaviour
{
    [SerializeField] private LampStatsManager _lampStatsManager;
    private bool _isReadyToCollect = false;
    private float _waveTime;
    private CustomEvent _waveEndEvent;
    private CustomEvent _lampDamageEvent;
    private CustomEvent _healthUpgradeEvent;
    private CustomEvent _coolUpgradeEvent;

    private void OnEnable()
    {
        EnemyManager.OnWaveStarted += StartTimer;
        EnemyManager.OnWaveEnded += SubmitWaveEndEvent;
        Lamp.OnLampDamaged += SubmitLampDamageEvent;
        _lampStatsManager.OnHealthChange += SubmitHealthUpgradeEvent;
        _lampStatsManager.OnCooldownChange += SubmitCoolUpgradeEvent;
        
        _waveEndEvent = new CustomEvent("waveFinished");
        _lampDamageEvent = new CustomEvent("LampDamaged");
        _healthUpgradeEvent = new CustomEvent("healthUpgrade");
        _coolUpgradeEvent = new CustomEvent("coolUpgrade");
    }
    
    private void OnDisable()
    {
        EnemyManager.OnWaveStarted -= StartTimer;
        EnemyManager.OnWaveEnded -= SubmitWaveEndEvent;
        Lamp.OnLampDamaged -= SubmitLampDamageEvent;
        _lampStatsManager.OnHealthChange -= SubmitHealthUpgradeEvent;
        _lampStatsManager.OnCooldownChange -= SubmitCoolUpgradeEvent;
    }

    public void AllowDataCollection()
    {
        _isReadyToCollect = true;
    }
    
    public void RefuseDataCollection()
    {
        _isReadyToCollect = true;
    }
    
    private void StartTimer(int wave)
    {
        _waveTime = Time.time;
    }

    private void SubmitWaveEndEvent(int wave)
    {
        _waveTime = Time.time - _waveTime;
        if (_isReadyToCollect)
        {
            _waveEndEvent.Reset();
            _waveEndEvent.Add("waveNum", wave);
            _waveEndEvent.Add("waveTime", _waveTime);
            AnalyticsService.Instance.RecordEvent(_waveEndEvent);
            
        }
    }

    private void SubmitLampDamageEvent(EnemyBase enemy)
    {
        if (_isReadyToCollect)
        {
            _lampDamageEvent.Reset();
            _lampDamageEvent.Add("enemyType", enemy.EnemyType.ToString());
            AnalyticsService.Instance.RecordEvent(_lampDamageEvent);
        }
    }
    
    private void SubmitHealthUpgradeEvent()
    {
        if (_isReadyToCollect)
        {
            _healthUpgradeEvent.Reset();
            AnalyticsService.Instance.RecordEvent(_healthUpgradeEvent);
        }
    }
    
    private void SubmitCoolUpgradeEvent()
    {
        if (_isReadyToCollect)
        {
            _coolUpgradeEvent.Reset();
            AnalyticsService.Instance.RecordEvent(_coolUpgradeEvent);
        }
    }
}
