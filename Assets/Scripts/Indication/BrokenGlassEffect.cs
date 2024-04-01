using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrokenGlassEffect : MonoBehaviour
{
    [SerializeField] private Image _brokenGlassImage;
    private bool _isActive = false;
    private readonly float _damageDuration = 0.25f;
    private readonly float _deathDuration = 0.75f;
    private float _currentDuration;
    private float _localTime;
    
    public void Play(BrokenGlassEventTypes eventType)
    {
        switch (eventType)
        {
            case BrokenGlassEventTypes.Damage:
                _currentDuration = _damageDuration;
                break;
            case BrokenGlassEventTypes.Death:
                _currentDuration = _deathDuration;
                break;
        }
        _isActive = true;
        _localTime = 0;
    }
    
    void Update()
    {
        if (_isActive)
        {
            float phase = _localTime / _currentDuration;
            if(phase > 1)
            {
                _isActive = false;
                _brokenGlassImage.color = new Color(1, 1, 1, 0);
                return;
            }
            _brokenGlassImage.color = new Color(1, 1, 1, (1-phase)*0.35f);
            _localTime += Time.deltaTime;
        }
    }
}
