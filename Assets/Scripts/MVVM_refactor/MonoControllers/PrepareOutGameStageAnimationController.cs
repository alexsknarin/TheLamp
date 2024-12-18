using System;
using UnityEngine;

public class PrepareOutGameStageAnimationController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool _skip = false;   
    [SerializeField] private float _duration;
    [SerializeField] private TextFader _waveText;
    
    private float _localTime;
    private bool _isPlaying;
    public event Action OnFinishedEvent;
    
    public void Play()
    {
        _waveText.SetVisibilityLevel(1);
        
        if (_skip)
        {
            SetFinalState();
            return;
        }
        _localTime = 0;
        _isPlaying = true;
    }

    private void SetFinalState()
    {
        _waveText.SetVisibilityLevel(0);
        _waveText.gameObject.SetActive(false);
        OnFinishedEvent?.Invoke();
    }

    private void Update()
    {
        if(_isPlaying) 
        {
            float phase = _localTime / _duration;
            if (phase > 1)
            {
                _isPlaying = false;
                SetFinalState();
                return;
            }
            _waveText.SetVisibilityLevel(1-phase);
            _localTime += Time.deltaTime;
        }
    }
}
