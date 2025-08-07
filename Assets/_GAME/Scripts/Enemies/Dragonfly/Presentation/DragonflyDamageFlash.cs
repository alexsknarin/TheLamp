using System;
using System.Collections;
using System.Collections.Generic;
using _GAME.Scripts.Enemies.Generic.Presentation;
using UnityEngine;
using UnityEngine.VFX;

namespace _GAME.Scripts.Enemies.Dragonfly.Presentation
{
    public class DragonflyDamageFlash : DamageIndication
    {
        [SerializeField] private List<MeshRenderer> _meshRenderer;
        [SerializeField] private float _duration = 1.2f;
        [SerializeField] private VisualEffect _damageParticles;
        private List<Material> _materials = new ();
        private WaitForSeconds _damageFlashDuration;
        private Transform _contactCollisionTransform;
        private float _localTime;
        
        public override void Initialize()
        {
            foreach (var meshRenderer in _meshRenderer)
            {
                if (meshRenderer == null) continue;
                var material = meshRenderer.material;
                _materials.Add(material);
                material.SetFloat("_AttackSemaphore", 0f);
            }
            
            _damageFlashDuration = new WaitForSeconds(_duration);
            enabled = false;
            _damageParticles.gameObject.SetActive(false);
        }

        public void Reset()
        {
            SetAttackSemaphore(0);
        }

        public override void Play()
        {
            enabled = true;
            _damageParticles.gameObject.SetActive(true);
            _localTime = 0;
        
            if (_contactCollisionTransform != null)
            {
                _damageParticles.transform.localPosition = _contactCollisionTransform.localPosition;
                Vector3 direction = -_contactCollisionTransform.position.normalized;
                _damageParticles.SetVector3("Direction", direction);
                _damageParticles.SendEvent("OnDamage");
            }
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
                SetDamageMaterialPhase(0);
                return;
            }
            SetDamageMaterialPhase(1 - phase);
            _localTime += Time.deltaTime;
        }

        private void SetAttackSemaphore(float value)
        {
            for (int i=0; i < _materials.Count; i++)
            {
                _materials[i].SetFloat("_AttackSemaphore", value);
            }
        }
        
        private void SetDamageMaterialPhase(float phase)
        {
            for (int i=0; i < _materials.Count; i++)
            {
                _materials[i].SetFloat("_DamageFade", phase);
            }
        }
    }
}
