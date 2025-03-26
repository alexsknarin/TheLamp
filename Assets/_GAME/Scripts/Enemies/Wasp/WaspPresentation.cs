using System.Collections;
using _GAME.Scripts.Lib;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;

namespace _GAME.Scripts.Enemies.Wasp
{
    public class WaspPresentation : MonoBehaviour, IInitializable
    {
        [SerializeField] private Wasp _wasp;
        [SerializeField] private WaspAnimationClipEventListener _animationClipEventListener;
        [SerializeField] private MeshRenderer _waspBodyMeshRenderer;
        [SerializeField] private TrailResetHandler _trailResetHandler;
        [SerializeField] private VisualEffect _damageParticles;
        [SerializeField] private VisualEffect _damageEmitParticles;
        [SerializeField] private float _deathDuration;
        private bool _isDead;
        private bool _isDamaged;
        private Material _waspBodyMaterial;
        private WaitForSeconds _damageFlashDuration = new WaitForSeconds(0.8f);
        private float _localTime;
        
        private float _currentHealth;
        private float _maxHealth;
        private float _damagePhase;
    
        public void Initialize()
        {
            _wasp.HealthChanged += OnHealthChanged;
            _wasp.Dead += OnDead;
            _wasp.Damaged += OnDamaged;
            _animationClipEventListener.TrailReset += ResetTrail;
        
            _waspBodyMeshRenderer.gameObject.SetActive(true);
            _localTime = 0;
            _isDead = false;
            _waspBodyMaterial = _waspBodyMeshRenderer.material;
            _waspBodyMaterial.SetFloat("_DamagePhase", 0);
            _waspBodyMaterial.SetInt("_isDamaged", 0);
            _waspBodyMaterial.SetFloat("_DeathPhase", 0);
        
            _damageEmitParticles.SendEvent("OnEndEmit");
            _damageEmitParticles.SetFloat("Rate", 0);
        }

        private void OnDestroy()
        {
            _wasp.HealthChanged -= OnHealthChanged;
            _wasp.Dead -= OnDead;
            _wasp.Damaged -= OnDamaged;
            _animationClipEventListener.TrailReset -= ResetTrail;
        }

        private void OnHealthChanged(int currentHealth, int maxHealth)
        {
            _currentHealth = currentHealth;
            _maxHealth = maxHealth;
            _damagePhase = 1 - (float)currentHealth/maxHealth;
            if (_damagePhase > 0)
            {
                _damagePhase += 0.2f;
            }
            
            _waspBodyMaterial.SetFloat("_DamagePhase", _damagePhase);
        }

        private void OnDamaged()
        {
            _waspBodyMaterial.SetInt("_isDamaged", 1);
            float damagePhase = Mathf.Clamp(_damagePhase, 0, 1);
            float emitRate = damagePhase * 22;
            _damageEmitParticles.SendEvent("OnStartEmit");
            _damageEmitParticles.SetFloat("Rate", emitRate);
            StartCoroutine(WaitForDamageFlashEnd());
        }

        private IEnumerator WaitForDamageFlashEnd()
        {
            yield return _damageFlashDuration;
            _waspBodyMaterial.SetInt("_isDamaged", 0);
        }

        private void OnDead()
        {
            _isDead = true;
            _localTime = 0;
            _damageEmitParticles.SendEvent("OnStartEmit");
            _damageEmitParticles.SetFloat("Rate", 35);
        }

        private void ResetTrail()
        {
            _trailResetHandler.Initialize();
        }

        private void PerformDeath()
        {
            if (_isDead)
            {
                float phase = _localTime / _deathDuration;
                _waspBodyMaterial.SetFloat("_DeathPhase", phase);
            
                if (phase > 1)
                {
                    _isDead = false;
                    _waspBodyMaterial.SetFloat("_DeathPhase", 1);
                    _damageEmitParticles.SendEvent("OnEndEmit");
                    _damageEmitParticles.SetFloat("Rate", 0);
                    _trailResetHandler.Initialize();
                    return;
                }
            
                _localTime += Time.deltaTime;
            }
        }

        private void Update()
        {
            PerformDeath();
        }

        private void Reset()
        {
            _waspBodyMeshRenderer.gameObject.SetActive(false);
            _trailResetHandler.Initialize();
        }
    }
}
