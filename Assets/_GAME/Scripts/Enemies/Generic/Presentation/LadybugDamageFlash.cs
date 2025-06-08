using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace _GAME.Scripts.Enemies.Generic.Presentation
{
    public class LadybugDamageFlash : DamageIndication
    {
        [SerializeField] private List<MeshRenderer> _meshRenderer;
        [SerializeField] private MeshRenderer _attackZone;
        [SerializeField] private float _duration = 0.5f;
        [SerializeField] private VisualEffect _damageParticles;
        private List<Material> _bodyMaterials = new ();
        private Material _attackZoneMaterial;
        private float _localTime;

        public override void Initialize()
        {
            foreach (var meshRenderer in _meshRenderer)
            {
                if (meshRenderer == null) continue;
                var material = meshRenderer.material;
                _bodyMaterials.Add(material);
                material.SetFloat("_DamageFade", 0f);
            }
            
            enabled = false;
            _attackZoneMaterial = _attackZone.material;
            _attackZoneMaterial.SetFloat("_Alpha", 0f);
        }

        public override void Play()
        {
            enabled = true;
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
                _attackZoneMaterial.SetFloat("_Alpha", 0f);
                return;
            }
            SetDamageMaterialPhase(1 - phase);
            _attackZoneMaterial.SetFloat("_Alpha", 1-Mathf.Clamp(phase*3f, 0, 1));
            _localTime += Time.deltaTime;
        }
        
        private void SetDamageMaterialPhase(float phase)
        {
            for (int i=0; i < _bodyMaterials.Count; i++)
            {
                _bodyMaterials[i].SetFloat("_DamageFade", phase);
            }
        }
    }
}