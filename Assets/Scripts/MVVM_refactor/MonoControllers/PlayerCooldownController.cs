using System;
using UnityEngine;

// TODO: add tickable interface remove update method and monobehaviour
public class PlayerCooldownController : MonoBehaviour
{
    private float _power;
    public float Power 
    {
        get => _power;
        private set
        {
            _power = value;
            OnPowerChangedEvent?.Invoke(_power);
        }
    } 
    public event Action<float> OnPowerChangedEvent;
    public event Action OnCooldownEndedEvent;
    
    private float _duration;
    private float _localTime;
    private bool _isPlaying = false;
    
    public void SetDuration(float duration)
    {
        _duration = duration;
    }
    
    public void StartCooldown()
    {
        Power = 0;
        _localTime = 0;
        _isPlaying = true;
    }
    
    public void StopCooldown()
    {
        _isPlaying = false;
    }

    private void PerformCooldown()
    {
        float phase = _localTime / _duration;
        if (phase > 1)
        {
            _isPlaying = false;
            Power = 1;
            OnCooldownEndedEvent?.Invoke();
            return;
        }
        Power = phase;
        _localTime += Time.deltaTime;
    }
    
    private void Update()
    {
        if (_isPlaying)
        {
            PerformCooldown();
        }
    }
}
