using TMPro;
using UnityEngine;

public class UiText : MonoBehaviour
{
    [SerializeField] private TMP_Text _uiText;
    [SerializeField] private float _fadeDuration;
    private float _localTime;
    private bool _isRevealing = false;
    private bool _isHiding = false;
    
    private Color _visibleColor = new Color(1, 1, 1, 1);
    private Color _invisibleColor = new Color(1, 1, 1, 0);

    public void ShowWaveText(string text)
    {
        _uiText.text = text;
        _uiText.enabled = true;
        _isRevealing = true;
        _localTime = 0;
    }
    
    public void HideWaveText()
    {
        _isHiding = true;
        _localTime = 0;
    }

    public void DisableText()
    {
        _uiText.enabled = false;
    }
    
    private void Update()
    {
        if (_isRevealing)
        {
            float phase = _localTime / _fadeDuration;
            if(phase > 1)
            {
                _isRevealing = false;
                _localTime = 0;
                _uiText.color = _visibleColor;
                return;
            }
            _uiText.color = Color.Lerp(_invisibleColor, _visibleColor, phase);
            
            _localTime += Time.deltaTime;
        }
        
        if (_isHiding)
        {
            float phase = _localTime / _fadeDuration;
            if(phase > 1)
            {
                _isHiding = false;
                _localTime = 0;
                _uiText.color = _visibleColor;
                _uiText.enabled = false;
                return;
            }
            _uiText.color = Color.Lerp(_visibleColor, _invisibleColor, phase);
            _localTime += Time.deltaTime;
        }
    }
}
