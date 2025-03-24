using System;
using _GAME.Scripts.Lib.Interfaces;
using _GAME.Scripts.UI.ViewModels;
using UnityEngine;
using UnityEngine.Rendering;

namespace _GAME.Scripts.InGamePresentation.Views
{
    public class LevelUpEffectView : MonoBehaviour, IInitializable
    {
        [SerializeField] private Volume _postProcessingVolume;
        [SerializeField] private float _duration;
        [SerializeField] private AnimationCurve _exposureCurve;
        [SerializeField] private float _highExposureValue;
        private PlayerGameplayViewModel _playerGameplayViewModel;
        
        private UnityEngine.Rendering.Universal.ColorAdjustments _colorAdjustments;
        private float _baseExposure;
        private float _highExposure;
        private float _localTime;

        public void Bind(PlayerGameplayViewModel playerGameplayViewModel)
        {
            _playerGameplayViewModel = playerGameplayViewModel;
            _playerGameplayViewModel.UpgradePointsChanged += Play;
        }

        private void OnDestroy()
        {
            _playerGameplayViewModel.UpgradePointsChanged -= Play;
        }

        public void Initialize()
        {
            VolumeProfile volumeProfile = _postProcessingVolume.profile;
            if (!volumeProfile)
            {
                throw new NullReferenceException(nameof(VolumeProfile));
            }

            if (!volumeProfile.TryGet(out _colorAdjustments))
            {
                throw new NullReferenceException(nameof(_colorAdjustments));
            }
            
            enabled = false;
        }

        private void Play()
        {
            _baseExposure = _colorAdjustments.postExposure.value;
            _highExposure = _baseExposure + _highExposureValue;
            _localTime = 0;
            enabled = true;
        }

        private void Update()
        {
            float phase = _localTime / _duration;
            
            if (phase >= 1.0f)
            {
                enabled = false;
                _colorAdjustments.postExposure.Override(_baseExposure);
                return;
            }
            float exposure = Mathf.Lerp(_baseExposure, _highExposure, _exposureCurve.Evaluate(phase));
            _colorAdjustments.postExposure.Override(exposure);
            
            _localTime += Time.deltaTime;
            
        }
    }
}
