using _GAME.Scripts.Enemies.Generic.Presentation;
using UnityEngine;
using UnityEngine.VFX;

namespace _GAME.Scripts.Enemies.Dragonfly.Presentation
{
    public class DragonflyDeathFlash : DamageIndication
    {
        [SerializeField] private MeshRenderer _bodyMeshRenderer;
        [SerializeField] private MeshRenderer _wingsMeshRenderer;
        [SerializeField] private float _duration = 1.7f;
        [SerializeField] private VisualEffect _deathParticles;
        [SerializeField] private VisualEffect _damageParticles;
        private Material _bodyMaterial;
        private Material _wingsMaterial;
        private float _localTime;
        private Transform _contactCollisionTransform;

        public override void Initialize()
        {
            enabled = false;
        
            _bodyMaterial = _bodyMeshRenderer.material;
            _wingsMaterial = _wingsMeshRenderer.material;
        
            _bodyMaterial.SetFloat("_DeathPhase", 0f);
            _wingsMaterial.SetFloat("_DeathPhase", 0f);
        
            _bodyMaterial.SetFloat("_AttackSemaphore", 0f);
            _wingsMaterial.SetFloat("_AttackSemaphore", 0f);
        
            _deathParticles.gameObject.SetActive(false);
        }

        public override void Play()
        {
            enabled = true;
            _localTime = 0;
            _bodyMaterial.SetFloat("_DeathPhase", 0f);
            _wingsMaterial.SetFloat("_DeathPhase", 0f);
            _bodyMaterial.SetFloat("_AttackSemaphore", 0f);
            _wingsMaterial.SetFloat("_AttackSemaphore", 0f);
        
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
                _bodyMaterial.SetFloat("_DeathPhase", 1f);
                _wingsMaterial.SetFloat("_DeathPhase", 1f);
            
                _deathParticles.SendEvent("OnDeathStart");
                _deathParticles.gameObject.SetActive(false);
                return;
            }
            _bodyMaterial.SetFloat("_DeathPhase", phase);
            _wingsMaterial.SetFloat("_DeathPhase", phase);
        
            _localTime += Time.deltaTime;
        }
    }
}