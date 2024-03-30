using System.Collections.Generic;
using Unity.Services.Analytics;
using UnityEngine;

public class AnalyticsCollector : MonoBehaviour
{
    private bool _isReadyToCollect = false;
    private float _waveTime;
    private CustomEvent _waveEndEvent;
    private CustomEvent _lampDamageEvent;

    private void OnEnable()
    {
        EnemyManager.OnWaveStarted += StartTimer;
        EnemyManager.OnWaveEnded += SubmitWaveEndEvent;
        Lamp.OnLampDamaged += SubmitLampDamageEvent;
        
        _waveEndEvent = new CustomEvent("waveFinished");
        _lampDamageEvent = new CustomEvent("LampDamaged");
    }
    
    private void OnDisable()
    {
        EnemyManager.OnWaveStarted -= StartTimer;
        EnemyManager.OnWaveEnded -= SubmitWaveEndEvent;
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
}
