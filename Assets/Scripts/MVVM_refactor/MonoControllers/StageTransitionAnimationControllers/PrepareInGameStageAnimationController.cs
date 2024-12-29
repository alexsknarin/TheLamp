using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class PrepareInGameStageAnimationController : MonoBehaviour
{
    private readonly Color HINT_TEXT_FULL_COLOR = new Color(1, 1, 1, 0.21f);
    private readonly Color HINT_TEXT_OFF_COLOR = new Color(1, 1, 1, 0.0f);
    
    [Header("Settings")]
    [SerializeField] private bool _skip = false;   
    [SerializeField] private float _duration;
    [SerializeField] private TextFader _waveText;
    [SerializeField] private GameObject _upgradeButtonsPanel;
    [FormerlySerializedAs("_upgradeHealthButton")] [SerializeField] private FadableButtonPresentation _upgradeHealthButtonPresentation;
    [FormerlySerializedAs("_upgradeAttackButton")] [SerializeField] private FadableButtonPresentation _upgradeAttackButtonPresentation;
    [FormerlySerializedAs("_upgradeCooldownButton")] [SerializeField] private FadableButtonPresentation _upgradeCooldownButtonPresentation;
    [SerializeField] private TMP_Text _hintText1;
    [SerializeField] private TMP_Text _hintText2;
    [SerializeField] private TMP_Text _hintText3;
    
    private float _localTime;
    private bool _isPlaying;
    private bool _isUpgradeRequired;
    public event Action OnFinishedEvent;
    
    public void Play(bool isUpgradeRequired, int waveNum)
    {
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
