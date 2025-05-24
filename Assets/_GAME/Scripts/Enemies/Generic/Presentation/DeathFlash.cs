using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace _GAME.Scripts.Enemies.Generic.Presentation
{
    public class DeathFlash : DamageIndication
    {
        [SerializeField] private List<MeshRenderer> _meshRenderer;
        [SerializeField] private float _duration = 1.7f;
        [SerializeField] private VisualEffect _deathParticles;
        [SerializeField] private VisualEffect _damageParticles;
        private List<Material> _materials = new ();
        private float _localTime;

        public override void Initialize()
        {
            enabled = false;
            
            foreach (var meshRenderer in _meshRenderer)
            {
                if (meshRenderer == null) continue;
                var material = meshRenderer.material;
                _materials.Add(material);
                material.SetFloat("_DeathFade", 0f);
                material.SetFloat("_Damage", 1f);
            }
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
            for (int i=0; i < _materials.Count; i++)
            {
                _materials[i].SetFloat("_DeathFade", phase);
            }
        }
    }
}