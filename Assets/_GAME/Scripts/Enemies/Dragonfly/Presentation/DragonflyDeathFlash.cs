using System;
using System.Collections.Generic;
using _GAME.Scripts.Enemies.Generic.Presentation;
using UnityEngine;
using UnityEngine.VFX;

namespace _GAME.Scripts.Enemies.Dragonfly.Presentation
{
    public class DragonflyDeathFlash : DamageIndication
    {
        [SerializeField] private List<MeshRenderer> _meshRenderer;
        [SerializeField] private float _duration = 1.7f;
        [SerializeField] private VisualEffect _deathParticles;
        [SerializeField] private VisualEffect _damageParticles;
        private List<Material> _materials = new ();
        private float _localTime;
        private Transform _contactCollisionTransform;

        public override void Initialize()
        {
            foreach (var meshRenderer in _meshRenderer)
            {
                if (meshRenderer == null) continue;
                var material = meshRenderer.material;
                _materials.Add(material);
                material.SetFloat("_DeathFade", 0f);
            }
            
            enabled = false;
            Reset();
        }

        public void Reset()
        {
            SetDamageMaterialPhase(0);
        
            _deathParticles.gameObject.SetActive(false);
        }

        public override void Play()
        {
            enabled = true;
            _localTime = 0;
            
            SetDamageMaterialPhase(0);
        
            if (_contactCollisionTransform != null)
            {
                _damageParticles.transform.localPosition = _contactCollisionTransform.localPosition;
                Vector3 direction = -_contactCollisionTransform.position.normalized;
                _damageParticles.SetVector3("Direction", direction);
                _damageParticles.SendEvent("OnDamage");
            }
       
            _deathParticles.gameObject.SetActive(true);
            _deathParticles.SendEvent("OnDeathStart");
        }

        public void SetContactCollisionTransform(Transform contactCollisionTransform)
        {
            _contactCollisionTransform = contactCollisionTransform;
        }

        private void Update()
        {
            float phase = _localTime / _duration;
            if (phase > 1)
            {
                enabled = false;
                SetDamageMaterialPhase(1f);
            
                _deathParticles.SendEvent("OnDeathStart");
                _deathParticles.gameObject.SetActive(false);
                return;
            }
            SetDamageMaterialPhase(phase);
        
            _localTime += Time.deltaTime;
        }
        
        private void SetDamageMaterialPhase(float phase)
        {
            for (int i=0; i < _materials.Count; i++)
            {
                _materials[i].SetFloat("_DeathFade", phase);
            }
        }
    }
}