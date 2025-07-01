using System.Collections;
using System.Collections.Generic;
using _GAME.Scripts.Lib;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.VFX;

namespace _GAME.Scripts.Enemies.Wasp
{
    public class WaspPresentation : MonoBehaviour, IInitializable
    {
        private static readonly int DeathFade = Shader.PropertyToID("_DeathFade");
        private static readonly int Health = Shader.PropertyToID("_Health");
        private static readonly int IsDamaged = Shader.PropertyToID("_isDamaged");
        private static readonly int StartFly = Animator.StringToHash("StartFly");
        private static readonly int AttackSemaphore = Shader.PropertyToID("_AttackSemaphore");
        private static readonly int StopFly = Animator.StringToHash("StopFly");
        [SerializeField] private Wasp _wasp;
        [SerializeField] private WaspMovement _movement;
        [SerializeField] private WaspAnimationClipEventListener _animationClipEventListener;
        [SerializeField] private MeshRenderer _waspBodyMeshRenderer;
        [SerializeField] private GameObject _wings;
        [SerializeField] private TrailResetHandler _trailResetHandler;
        [SerializeField] private VisualEffect _damageParticles;
        [SerializeField] private VisualEffect _damageEmitParticles;
        [SerializeField] private Transform _bodyTransform;
        [SerializeField] private float _deathDuration;
        [SerializeField] private Animator _wingsAnimator;
        private bool _isDead;
        private bool _isDamaged;
        private Material _waspBodyMaterial;
        private WaitForSeconds _damageFlashDuration = new WaitForSeconds(0.8f);
        private WaitForSeconds _preattackFlashDuration = new WaitForSeconds(0.2f);
        private float _localTime;
        
        private float _damagePhase;
    
        public void Initialize()
        {
            _wasp.Started += OnStarted;
            _wasp.HealthChanged += OnHealthChanged;
            _wasp.Dead += OnDead;
            _wasp.Damaged += OnDamaged;

            _movement.DeathStateStarted += OnDeathStateStarted;
            _movement.FailStateStarted += OnFailStateStarted;
            
            _animationClipEventListener.TrailReset += ResetTrail;
            _animationClipEventListener.StartFlying += OnStartFlying;
            _animationClipEventListener.PreAttackStarted += OnPreAttackStarted;

            _waspBodyMaterial = _waspBodyMeshRenderer.sharedMaterial;
            _wings.SetActive(true);
        }

        private void OnDestroy()
        {
            _wasp.Started -= OnStarted;
            _wasp.HealthChanged -= OnHealthChanged;
            _wasp.Dead -= OnDead;
            _wasp.Damaged -= OnDamaged;
            
            _movement.DeathStateStarted -= OnDeathStateStarted;
            _movement.FailStateStarted -= OnFailStateStarted;
            
            _animationClipEventListener.TrailReset -= ResetTrail;
            _animationClipEventListener.StartFlying -= OnStartFlying;
            _animationClipEventListener.PreAttackStarted -= OnPreAttackStarted; 
            
            _waspBodyMaterial.SetFloat(DeathFade, 0);
            _wings.SetActive(true);
        }

        private void OnStarted()
        {
            _waspBodyMeshRenderer.gameObject.SetActive(true);
            _localTime = 0;
            _isDead = false;
            
            _waspBodyMaterial.SetFloat(Health, 1);
            _waspBodyMaterial.SetInt(IsDamaged, 0);
            _waspBodyMaterial.SetFloat(DeathFade, 0);
            
            _wingsAnimator.SetTrigger(StartFly);
        
            _damageEmitParticles.SendEvent("OnEndEmit");
            _damageEmitParticles.SetFloat("Rate", 0);
        }

        private void OnHealthChanged(int currentHealth, int maxHealth)
        {
            _damagePhase = (float)currentHealth/maxHealth;
            if (_damagePhase > 0)
            {
                _damagePhase += 0.2f;
            }
            
            _waspBodyMaterial.SetFloat(Health, _damagePhase);
        }

        private void OnDamaged()
        {
            _waspBodyMaterial.SetInt(IsDamaged, 1);
            float damagePhase = Mathf.Clamp(_damagePhase, 0, 1);
            float emitRate = damagePhase * 22;
            _damageEmitParticles.SendEvent("OnStartEmit");
            _damageEmitParticles.SetFloat("Rate", emitRate);
            StartCoroutine(WaitForDamageFlashEnd());
            
            EmitDamageParticles();
        }

        private IEnumerator WaitForPreattackEnd()
        {
            yield return _preattackFlashDuration;
            _waspBodyMaterial.SetInt(AttackSemaphore, 0);
        }

        private IEnumerator WaitForDamageFlashEnd()
        {
            yield return _damageFlashDuration;
            _waspBodyMaterial.SetInt(IsDamaged, 0);
        }

        private void OnDead()
        {
            _isDead = true;
            _localTime = 0;
            _damageEmitParticles.SendEvent("OnStartEmit");
            _damageEmitParticles.SetFloat("Rate", 35);
            _wings.SetActive(false);
            EmitDamageParticles();
        }

        private void EmitDamageParticles()
        {
            Vector3 direction = _bodyTransform.position.normalized;
            _damageParticles.SetVector3("Direction", direction);
            _damageParticles.SendEvent("OnDamage");
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
                _waspBodyMaterial.SetFloat(DeathFade, phase);
            
                if (phase > 1)
                {
                    _isDead = false;
                    _waspBodyMaterial.SetFloat(DeathFade, 1);
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

        private void OnDeathStateStarted()
        {
            _wingsAnimator.SetTrigger(StopFly);
        }

        private void OnFailStateStarted()
        {
            _wingsAnimator.SetTrigger(StopFly);
        }

        private void OnStartFlying()
        {
            _wingsAnimator.SetTrigger(StartFly);
        }

        private void OnPreAttackStarted()
        {
            _waspBodyMaterial.SetInt(AttackSemaphore, 1);
            StartCoroutine(WaitForPreattackEnd());
        }
    }
}
