using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningFlash : MonoBehaviour
{
    [SerializeField] private Light _light;  
    [SerializeField] private float _flashDuration;
    [SerializeField] private AnimationCurve _flashCurve;
    [SerializeField] private float _lightMaxIntensity;
    private float _localTime;
    private bool _isPlaying = false;

    private void OnEnable()
    {
        EnemyManager.OnBossAppear += Play;
        EnemyManager.OnBossDeath += Play;
    }
    
    private void OnDisable()
    {
        EnemyManager.OnBossAppear -= Play;
        EnemyManager.OnBossDeath -= Play;
    }
    
    private void Start()
    {
        _light.enabled = false;
    }

    private void Play()
    {
        _light.enabled = true;
        _light.intensity = 0;
        _localTime = 0;
        _isPlaying = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_isPlaying)
        {
            float phase = _localTime / _flashDuration;
            if (phase > 1)
            {
                _light.intensity = 0;        
                _isPlaying = false;
                _light.enabled = false;
            }
            
            _light.intensity = _flashCurve.Evaluate(phase) * _lightMaxIntensity;
            _localTime += Time.deltaTime;
        }
    }
}
