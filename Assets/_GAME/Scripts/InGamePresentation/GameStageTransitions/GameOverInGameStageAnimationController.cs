using System;
using _GAME.Scripts.Lamp;
using _GAME.Scripts.UI.UiElements;
using UnityEngine;
using UnityEngine.Rendering;

namespace _GAME.Scripts.InGamePresentation.GameStageTransitions
{
    public class GameOverInGameStageAnimationController : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private float _lampDestructionDuration = 3;
        [SerializeField] private float _cameraStartZPosition = -5.88f;
        [SerializeField] private float _cameraEndZPosition = -7.1f;
        [SerializeField] private float _startExposure = 0;
        [SerializeField] private float _endExposure = -8;
        [Header("Scene Dependencies")]
        [Header("Scene")]
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private AnimationCurve _cameraAnimationCurve;
        [SerializeField] private Volume _postProcessingVolume;
        [Header("UI")]
        [SerializeField] private GameObject _gameOverUi;
        [SerializeField] private AnimationCurve _gameOverTextAnimationCurve;
        [SerializeField] private TextFader _gameOverText;
        [SerializeField] private AnimationCurve _gameOverButtonsAnimationCurve;
        [SerializeField] private FadableButtonPresentation _restartWithAdButton;
        [SerializeField] private FadableButtonPresentation _restartNoAdButton;
        [SerializeField] private float _restartNoAdButtonAdDisabledY = -503;
        [SerializeField] private float _exitButtonAdDisabledY = -685;
        [SerializeField] private float _restartNoAdButtonAdEnabledY = -263;
        [SerializeField] private float _exitButtonAdEnabledY = -445;
        [SerializeField] private FadableButtonPresentation _exitButton;
        [SerializeField] private GameObject _ingameUi;
        [Header("Lamp")]
        [SerializeField] private LampDeathAnimation _lampDeathAnimation;
        private bool _skip = false;   
        private float _duration;
        private UnityEngine.Rendering.Universal.ColorAdjustments _colorAdjustments;
        private float _localTime;

        public event Action GameoverInFinished;

        public void Initialize()
        {
            // Get Control over Exposure
            VolumeProfile volumeProfile = _postProcessingVolume.profile;
            if (!volumeProfile) throw new NullReferenceException(nameof(VolumeProfile));
            // You can leave this variable out of your function, so you can reuse it throughout your class.
            if (!volumeProfile.TryGet(out _colorAdjustments))
                throw new NullReferenceException(nameof(_colorAdjustments));
            _colorAdjustments.postExposure.Override(_startExposure);
        
            _lampDeathAnimation.Initialize();
            enabled = false;
        }

        public void Play(bool skip, bool isAdNeeded, float duration, Vector3 enemyPosition)
        {
            _skip = skip;
            _duration = duration;
        
            if (_skip)
            {
                SetFinalState();
                return;
            }
        
            _gameOverUi.SetActive(true);
            _gameOverText.SetVisibilityLevel(0);
            _restartWithAdButton.SetVisibilityLevel(0);
            _restartNoAdButton.SetVisibilityLevel(0);
            _exitButton.SetVisibilityLevel(0);
            _ingameUi.SetActive(false);
        
            _lampDeathAnimation.Play(_lampDestructionDuration, enemyPosition);
        
            _localTime = 0;
            enabled = true;
        
            if (isAdNeeded)
            {
                _restartWithAdButton.gameObject.SetActive(true);
                var pos = _restartNoAdButton.transform.localPosition;
                pos.y = _restartNoAdButtonAdDisabledY;
                _restartNoAdButton.transform.localPosition = pos;
                pos = _exitButton.transform.localPosition;
                pos.y = _exitButtonAdDisabledY;
                _exitButton.transform.localPosition = pos;
            }
            else
            {
                _restartWithAdButton.gameObject.SetActive(false);
                var pos = _restartNoAdButton.transform.localPosition;
                pos.y = _restartNoAdButtonAdEnabledY;
                _restartNoAdButton.transform.localPosition = pos;
                pos = _exitButton.transform.localPosition;
                pos.y = _exitButtonAdEnabledY;
                _exitButton.transform.localPosition = pos;
            }
        }

        private void Update()
        {
            float phase = _localTime / _duration;
            if (phase > 1)
            {
                enabled = false;
                SetFinalState();
            }
        
            Vector3 cameraPosition = _cameraTransform.position;
            cameraPosition.z = Mathf.Lerp(_cameraStartZPosition, _cameraEndZPosition, _cameraAnimationCurve.Evaluate(phase));
            _cameraTransform.position = cameraPosition;
            _colorAdjustments.postExposure.Override(Mathf.Lerp(_startExposure, _endExposure, _cameraAnimationCurve.Evaluate(phase)));
        
            _gameOverText.SetVisibilityLevel(_gameOverTextAnimationCurve.Evaluate(phase));
        
            _restartWithAdButton.SetVisibilityLevel(_gameOverButtonsAnimationCurve.Evaluate(phase));
            _restartNoAdButton.SetVisibilityLevel(_gameOverButtonsAnimationCurve.Evaluate(phase));
            _exitButton.SetVisibilityLevel(_gameOverButtonsAnimationCurve.Evaluate(phase));
        
        
            _localTime += Time.deltaTime;
        }

        private void SetFinalState()
        {
            _localTime = 0;
        
            // Environment
            Vector3 cameraPosition = _cameraTransform.position;
            cameraPosition.z = _cameraEndZPosition;
            _cameraTransform.position = cameraPosition;
            _colorAdjustments.postExposure.Override(_endExposure);
            //UI
            _gameOverText.SetVisibilityLevel(1);
            _restartWithAdButton.SetVisibilityLevel(1);
            _restartNoAdButton.SetVisibilityLevel(1);
            _exitButton.SetVisibilityLevel(1);
        
            GameoverInFinished?.Invoke();
        }
    }
}
