using System;
using System.Collections.Generic;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;
using UnityEngine.VFX;

namespace _GAME.Scripts.Enemies.Dragonfly.Presentation
{
    public class DragonflyHealthIndication : MonoBehaviour, IInitializable
    {
        private static readonly int Health = Shader.PropertyToID("_Health");
        [SerializeField] private List<MeshRenderer> _meshRenderer;
        [SerializeField] private VisualEffect _damageEmitParticles;
        [SerializeField] private float _damageEmitRate = 22f;
        [SerializeField] private float _damageLifeMin = 0.1f;
        [SerializeField] private float _damageLifeMax = 0.45f;
        [SerializeField] private float _remapMax = 1f;
        private List<Material> _materials = new ();

        private bool _isParticleSystemActive = false;

        public void Initialize()
        {
            foreach (var meshRenderer in _meshRenderer)
            {
                if (meshRenderer == null) continue;
                var material = meshRenderer.material;
                _materials.Add(material);
                material.SetFloat(Health, 1f);
            }
  
            Reset();
        }

        public void Reset()
        {
            for (int i=0; i < _materials.Count; i++)
            {
                _materials[i].SetFloat(Health, 1f);
            }
            
            _isParticleSystemActive = false;
            _damageEmitParticles.SendEvent("OnEndEmit");
            _damageEmitParticles.SetFloat("Rate", 0);
            _damageEmitParticles.SetFloat("LifeMin", _damageLifeMin);
            _damageEmitParticles.SetFloat("LifeMax", _damageLifeMax);
            _damageEmitParticles.gameObject.SetActive(false);
        }

        public void Refresh(int currentHealth, int maxHealth)
        {
            SetHealth(currentHealth, maxHealth);
            
            float damagePhase = ((float)(maxHealth - currentHealth) / maxHealth) * 0.5f;

            if (damagePhase > 0.05f && !_isParticleSystemActive)
            {
                _isParticleSystemActive = true;
                _damageEmitParticles.gameObject.SetActive(true);
                _damageEmitParticles.SendEvent("OnStartEmit");
                _damageEmitParticles.SetFloat("Rate", damagePhase * _damageEmitRate);
            }

            if (_isParticleSystemActive)
            {
                _damageEmitParticles.SetFloat("Rate", damagePhase * _damageEmitRate);
            }
        }

        private void SetHealth(int currentHealth, int maxHealth)
        {
            for (int i=0; i < _materials.Count; i++)
            {
                _materials[i].SetFloat(Health, ((float)currentHealth / maxHealth) * _remapMax);
            }
        }
    }
}
