using System;
using TMPro;
using UnityEngine;

public class PrepareInGameStageAnimationController : MonoBehaviour
{
    private readonly Color HINT_TEXT_FULL_COLOR = new Color(1, 1, 1, 0.21f);
    private readonly Color HINT_TEXT_OFF_COLOR = new Color(1, 1, 1, 0.0f);
    
    [Header("Settings")]
    [SerializeField] private TextFader _waveText;
    [SerializeField] private GameObject _upgradeButtonsPanel;
    [SerializeField] private FadableButtonPresentation _upgradeHealthButtonPresentation;
    [SerializeField] private FadableButtonPresentation _upgradeAttackButtonPresentation;
    [SerializeField] private FadableButtonPresentation _upgradeCooldownButtonPresentation;
    [SerializeField] private TMP_Text _hintText1;
    [SerializeField] private TMP_Text _hintText2;
    [SerializeField] private TMP_Text _hintText3;
    private bool _skip = false;   
    private float _duration;
    private float _localTime;
    private bool _isPlaying;
    private bool _isUpgradeRequired;
    public event Action PrepareInFinished;
    
    public void Play(bool skip, float duration, bool isUpgradeRequired, int waveNum)
    {
        _skip = skip;
        _duration = duration;
        _isUpgradeRequired = isUpgradeRequired;
        
        _waveText.gameObject.SetActive(true);
        _waveText.SetText("Start Wave " + waveNum.ToString());
        _waveText.SetVisibilityLevel(0);

        if (_isUpgradeRequired)
        {
            _upgradeButtonsPanel.SetActive(true);
            _upgradeHealthButtonPresentation.SetVisibilityLevel(0);
            _upgradeAttackButtonPresentation.SetVisibilityLevel(0);
            _upgradeCooldownButtonPresentation.SetVisibilityLevel(0);
            _hintText1.color = HINT_TEXT_OFF_COLOR;
            _hintText2.color = HINT_TEXT_OFF_COLOR;
            _hintText3.color = HINT_TEXT_OFF_COLOR;    
        }
        
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
        _waveText.SetVisibilityLevel(1);
        if (_isUpgradeRequired)
        {
            _upgradeHealthButtonPresentation.SetVisibilityLevel(1);
            _upgradeAttackButtonPresentation.SetVisibilityLevel(1);
            _upgradeCooldownButtonPresentation.SetVisibilityLevel(1);
            _hintText1.color = HINT_TEXT_FULL_COLOR;
            _hintText2.color = HINT_TEXT_FULL_COLOR;
            _hintText3.color = HINT_TEXT_FULL_COLOR;    
        }
        PrepareInFinished?.Invoke();
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
            _waveText.SetVisibilityLevel(phase);
            if (_isUpgradeRequired)
            {
                _upgradeHealthButtonPresentation.SetVisibilityLevel(phase);
                _upgradeAttackButtonPresentation.SetVisibilityLevel(phase);
                _upgradeCooldownButtonPresentation.SetVisibilityLevel(phase);
                _hintText1.color = Color.Lerp(HINT_TEXT_OFF_COLOR, HINT_TEXT_FULL_COLOR, phase);
                _hintText2.color = Color.Lerp(HINT_TEXT_OFF_COLOR, HINT_TEXT_FULL_COLOR, phase);
                _hintText3.color = Color.Lerp(HINT_TEXT_OFF_COLOR, HINT_TEXT_FULL_COLOR, phase);   
            }
            _localTime += Time.deltaTime;
        }
    }
}
