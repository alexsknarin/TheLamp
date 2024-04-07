using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FakeAd : MonoBehaviour
{
    [SerializeField] private Image _bgImage;
    [SerializeField] private TMP_Text _adText1;
    [SerializeField] private TMP_Text _adText2;
    [SerializeField] private TMP_Text _adText3;
    [SerializeField] private TMP_Text _adText4;
    [SerializeField] private Transform _progressBaTransform;
    [SerializeField] private float _duration;
    
    public event Action OnAdFinished;
    
    private bool _isPlaying = false;
    private float _localTime;

    private Color _invisibleTextColor = new Color(1f, 1f, 1f, 0f);    
    private Color _visibleTextColor = new Color(1f, 1f, 1f, 1f);
    private Color _startBgColor = new Color(.213f, .196f, .106f, 1f);
    private Color _endBgColor = new Color(1f, 0.996f, 0.991f, 1f);

    public void Play()
    {
        gameObject.SetActive(true);
        UpdateProgressBar(0);
        _adText1.color = _invisibleTextColor;
        _adText2.color = _invisibleTextColor;
        _adText3.color = _invisibleTextColor;
        _adText4.color = _invisibleTextColor;
        _bgImage.color = _startBgColor;
        _localTime = 0;
        _isPlaying = true;
    }
    
    private void UpdateProgressBar(float progress)
    {
        Vector3 localScale = Vector3.one;
        localScale.x = progress;
        _progressBaTransform.localScale = localScale;
    }

    private void Update()
    {
        if (_isPlaying)
        {
            float phase = _localTime / _duration;
            if (phase > 1)
            {
                _isPlaying = false;
                gameObject.SetActive(false);
                OnAdFinished?.Invoke();
                return;
            }   
            _bgImage.color = Color.Lerp(_startBgColor, _endBgColor, Mathf.Clamp01(phase * 5));
            _adText1.color = Color.Lerp(_invisibleTextColor, _visibleTextColor, Mathf.Clamp01(phase * 4 - 0.1f));
            _adText2.color = Color.Lerp(_invisibleTextColor, _visibleTextColor, Mathf.Clamp01(phase * 4 - 0.7f));
            _adText3.color = Color.Lerp(_invisibleTextColor, _visibleTextColor, Mathf.Clamp01(phase * 4 - 1.4f));
            _adText4.color = Color.Lerp(_invisibleTextColor, _visibleTextColor, Mathf.Clamp01(phase * 4 - 2.1f));
            UpdateProgressBar(phase);
            _localTime += Time.deltaTime;
        }
    }
}
