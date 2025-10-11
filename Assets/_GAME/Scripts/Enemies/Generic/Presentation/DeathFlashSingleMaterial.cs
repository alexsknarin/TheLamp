using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace _GAME.Scripts.Enemies.Generic.Presentation
{
    public class DeathFlashSingleMaterial : DamageIndication
    {
        private static readonly int DeathFade = Shader.PropertyToID("_DeathFade");
        private static readonly int Damage = Shader.PropertyToID("_Damage");
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private float _duration = 1.7f;
        [SerializeField] private VisualEffect _deathParticles;
        [SerializeField] private VisualEffect _damageParticles;
        private Material _material;
        private float _localTime;

        public override void Initialize()
        {
            enabled = false;
            _material = _meshRenderer.sharedMaterial;
            _material.SetFloat(DeathFade, 0f);
            _material.SetFloat(Damage, 1f);
            _deathParticles.gameObject.SetActive(false);
        }

        public override void Play()
        {
            enabled = true;
            _localTime = 0;
            SetDamageMaterialPhase(0f);

            Vector3 direction = transform.position.normalized;
            _deathParticles.gameObject.SetActive(true);
            _damageParticles.SetVector3("Direction", direction);
            _deathParticles.SendEvent("OnDeathStart");
            _damageParticles.SendEvent("OnDamage");
        }

        private void Update()
        {
            float phase = _localTime / _duration;
            if (phase > 1)
            {
                enabled = false;
                SetDamageMaterialPhase(1f);
                _deathParticles.SendEvent("OnDeathStart");
                return;
            }
            SetDamageMaterialPhase(phase);
            _localTime += Time.deltaTime;
        }

        private void SetDamageMaterialPhase(float phase)
        {
            _material.SetFloat(DeathFade, phase);
        }
    }
}
