using System;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Lamp
{
    public class LampDamageAnimation : MonoBehaviour, IInitializable
    {
        [SerializeField] private LampEmissionController _lampEmissionController;
        private float _duration;
        private float _localTime = 0;

        public event Action Finished;

        public void Initialize()
        {
            enabled = false;
        }

        public void Play(float duration)
        {
            _lampEmissionController.IsDamageEnabled = true;
            _lampEmissionController.DamageMix = 1f;
        
            enabled = true;
            _duration = duration;
            _localTime = 0;
        }

        private void Update()
        {
            float phase = _localTime / _duration;
            if (phase > 1)
            {
                _lampEmissionController.DamageMix = 0f;
                _lampEmissionController.IsDamageEnabled = false;
                _lampEmissionController.Intensity = 0f;
                enabled = false;
                Finished?.Invoke();
                return;
            }
            _lampEmissionController.DamageMix = 1f - Mathf.Clamp(phase * 1.5f, 0, 1);
            _lampEmissionController.Intensity = 1f - phase;
        
            _localTime += Time.deltaTime;    
        }
    }
}
