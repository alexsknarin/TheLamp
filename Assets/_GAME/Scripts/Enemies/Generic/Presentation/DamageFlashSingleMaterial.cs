using UnityEngine;
using UnityEngine.VFX;

namespace _GAME.Scripts.Enemies.Generic.Presentation
{
    public class DamageFlashSingleMaterial : DamageIndication
    {
        private static readonly int DamageFade = Shader.PropertyToID("_DamageFade");
        [SerializeField] private Transform _transform;
        [SerializeField] private MeshRenderer _meshRenderer;
        [SerializeField] private float _duration = 0.5f;
        [SerializeField] private VisualEffect _damageParticles;
        private Material _material;
        private float _localTime;
        
        // TODO: correctly set material back on destroy
        
        public override void Initialize()
        {
            _material = _meshRenderer.sharedMaterial;
            _damageParticles.gameObject.SetActive(false);
            enabled = false;
        }

        public override void Play()
        {
            Debug.Log("Damaged!!!!");
            enabled = true;
            _damageParticles.gameObject.SetActive(true);
            
            _localTime = 0;
            Vector3 direction = _transform.position.normalized;
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
            _material.SetFloat(DamageFade, phase);
        }
    }
}
