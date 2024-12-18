using System;
using TMPro;
using UnityEngine;

public class PrepareOutGameStageAnimationController : MonoBehaviour
{
    private readonly Color HINT_TEXT_FULL_COLOR = new Color(1, 1, 1, 0.21f);
    private readonly Color HINT_TEXT_OFF_COLOR = new Color(1, 1, 1, 0.0f);
    
    [Header("Settings")]
    [SerializeField] private bool _skip = false;   
    [SerializeField] private float _duration;
    [SerializeField] private TextFader _waveText;
    [SerializeField] private GameObject _upgradeButtonsPanel;
    [SerializeField] private UpgradeButton _upgradeHealthButton;
    [SerializeField] private UpgradeButton _upgradeAttackButton;
    [SerializeField] private UpgradeButton _upgradeCooldownButton;
    [SerializeField] private TMP_Text _hintText1;
    [SerializeField] private TMP_Text _hintText2;
    [SerializeField] private TMP_Text _hintText3;
    
    private float _localTime;
    private bool _isPlaying;
    public event Action OnFinishedEvent;
    
    public void Play()
    {
        _waveText.SetVisibilityLevel(1);
        
        _upgradeHealthButton.SetVisibilityLevel(1);
        _upgradeAttackButton.SetVisibilityLevel(1);
        _upgradeCooldownButton.SetVisibilityLevel(1);
        _hintText1.color = HINT_TEXT_FULL_COLOR;
        _hintText2.color = HINT_TEXT_FULL_COLOR;
        _hintText3.color = HINT_TEXT_FULL_COLOR;
        
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
        _upgradeHealthButton.SetVisibilityLevel(0);
        _upgradeAttackButton.SetVisibilityLevel(0);
        _upgradeCooldownButton.SetVisibilityLevel(0);
        _hintText1.color = HINT_TEXT_OFF_COLOR;
        _hintText2.color = HINT_TEXT_OFF_COLOR;
        _hintText3.color = HINT_TEXT_OFF_COLOR;
        _upgradeButtonsPanel.SetActive(false);
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
            _upgradeHealthButton.SetVisibilityLevel(1-phase);
            _upgradeAttackButton.SetVisibilityLevel(1-phase);
            _upgradeCooldownButton.SetVisibilityLevel(1-phase);
            _hintText1.color = Color.Lerp(HINT_TEXT_OFF_COLOR, HINT_TEXT_FULL_COLOR, 1-phase);
            _hintText2.color = Color.Lerp(HINT_TEXT_OFF_COLOR, HINT_TEXT_FULL_COLOR, 1-phase);
            _hintText3.color = Color.Lerp(HINT_TEXT_OFF_COLOR, HINT_TEXT_FULL_COLOR, 1-phase);
            _localTime += Time.deltaTime;
        }
    }
}
