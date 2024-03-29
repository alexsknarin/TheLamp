using System.Collections.Generic;
using Unity.Services.Analytics;
using UnityEngine;

public class AnalyticsCollector : MonoBehaviour
{
    private bool _isReadyToCollect = false;
    private float _waveTime;

    private void OnEnable()
    {
        EnemyManager.OnWaveStarted += StartTimer;
        EnemyManager.OnWaveEnded += SubmitWaveEndEvent;
        Lamp.OnLampDamaged += SubmitLampDamageEvent;
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
            Dictionary<string, object> parameters = new Dictionary<string, object>()
            {
                {"waveNum", wave},
                {"waveTime", _waveTime}
            };
        
            AnalyticsService.Instance.CustomData("waveFinished", parameters);
        }
    }

    private void SubmitLampDamageEvent(EnemyBase enemy)
    {
        if (_isReadyToCollect)
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>()
            {
                {"enemyType", enemy.EnemyType.ToString()}
            };
        
            AnalyticsService.Instance.CustomData("LampDamaged", parameters);
        }
    }
}
