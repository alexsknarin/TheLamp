using System;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class PrepareOutGameStageAnimationController : MonoBehaviour, IInitializable
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
    public event Action PrepareOutFinished;

    public void Initialize()
    {
        enabled = false;
    }

    public void Play(bool skip, float duration)
    {
        _skip = skip;
        _duration = duration;
        _waveText.SetVisibilityLevel(1);
        
        _upgradeHealthButtonPresentation.SetVisibilityLevel(1);
        _upgradeAttackButtonPresentation.SetVisibilityLevel(1);
        _upgradeCooldownButtonPresentation.SetVisibilityLevel(1);
        _hintText1.color = HINT_TEXT_FULL_COLOR;
        _hintText2.color = HINT_TEXT_FULL_COLOR;
        _hintText3.color = HINT_TEXT_FULL_COLOR;
        
        if (_skip)
        {
            SetFinalState();
            return;
        }
        _localTime = 0;
        enabled = true;
    }

    private void SetFinalState()
    {
        _waveText.SetVisibilityLevel(0);
        _waveText.gameObject.SetActive(false);
        _upgradeHealthButtonPresentation.SetVisibilityLevel(0);
        _upgradeAttackButtonPresentation.SetVisibilityLevel(0);
        _upgradeCooldownButtonPresentation.SetVisibilityLevel(0);
        _hintText1.color = HINT_TEXT_OFF_COLOR;
        _hintText2.color = HINT_TEXT_OFF_COLOR;
        _hintText3.color = HINT_TEXT_OFF_COLOR;
        _upgradeButtonsPanel.SetActive(false);
        PrepareOutFinished?.Invoke();
    }

    private void Update()
    {
        float phase = _localTime / _duration;
        if (phase > 1)
        {
            enabled = false;
            SetFinalState();
            return;
        }
        _waveText.SetVisibilityLevel(1-phase);
        _upgradeHealthButtonPresentation.SetVisibilityLevel(1-phase);
        _upgradeAttackButtonPresentation.SetVisibilityLevel(1-phase);
        _upgradeCooldownButtonPresentation.SetVisibilityLevel(1-phase);
        _hintText1.color = Color.Lerp(HINT_TEXT_OFF_COLOR, HINT_TEXT_FULL_COLOR, 1-phase);
        _hintText2.color = Color.Lerp(HINT_TEXT_OFF_COLOR, HINT_TEXT_FULL_COLOR, 1-phase);
        _hintText3.color = Color.Lerp(HINT_TEXT_OFF_COLOR, HINT_TEXT_FULL_COLOR, 1-phase);
        _localTime += Time.deltaTime;
    }
}
