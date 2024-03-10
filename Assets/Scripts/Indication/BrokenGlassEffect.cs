using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BrokenGlassEffect : MonoBehaviour
{
    [SerializeField] private Image _brokenGlassImage;
    private bool _isActive = false;
    private float _duration = 0.25f;
    private float _prevTime;
    

    private void OnEnable()
    {
        Lamp.OnLampDamaged += Play;
    }
    
    private void OnDisable()
    {
        Lamp.OnLampDamaged -= Play;
    }
    
    private void Play()
    {
        _isActive = true;
        _prevTime = Time.time;
    }
    
    void Update()
    {
        if (_isActive)
        {
            float phase = (Time.time - _prevTime) / _duration;
            if(phase > 1)
            {
                _isActive = false;
                _brokenGlassImage.color = new Color(1, 1, 1, 0);
                return;
            }
            _brokenGlassImage.color = new Color(1, 1, 1, (1-phase)*0.055f);
        }
        
    }
}
