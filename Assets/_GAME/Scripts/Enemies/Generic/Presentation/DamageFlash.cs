using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace _GAME.Scripts.Enemies.Generic.Presentation
{
    public class DamageFlash : DamageIndication
    {
        [SerializeField] private List<MeshRenderer> _meshRenderer;
        [SerializeField] private float _duration = 0.5f;
        [SerializeField] private VisualEffect _damageParticles;
        private List<Material> _materials = new ();
        private float _localTime;

        public override void Initialize()
        {
            foreach (var meshRenderer in _meshRenderer)
            {
                if (meshRenderer == null) continue;
                var material = meshRenderer.material;
                _materials.Add(material);
                material.SetFloat("_DamageFade", 0f);
            }
            _damageParticles.gameObject.SetActive(false);
            enabled = false;
        }

        public override void Play()
        {
            enabled = true;
            _damageParticles.gameObject.SetActive(true);
            
            _localTime = 0;
            Vector3 direction = transform.position.normalized;
            _damageParticles.SetVector3("Direction", direction);
            _damageParticles.SendEvent("OnDamage");
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
        
        private void SetDamageMaterialPhase(float phase)
        {
            for (int i=0; i < _materials.Count; i++)
            {
                _materials[i].SetFloat("_DamageFade", phase);
            }
        }
    }
}
