using System.Collections;
using _GAME.Scripts.Enemies.Generic.Presentation;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megaspider
{
    public class MegaspiderPresentation : MonoBehaviour, IInitializable
    {
        [SerializeField] private Megaspider _megaspider;
        [SerializeField] private MegaspiderMovement _movement;
        [SerializeField] private MegaspiderAnimationClipEventListener _animationClipEvents;
        [SerializeField] private MegaspiderSpiderwebController _spiderwebController;
        [SerializeField] private DamageFlashSingleMaterial _damageFlash;
        [SerializeField] private PreAttackFlashSingleMaterial _preAttackFlash;
        [SerializeField] private float _preattackDuration = 0.15f;
        [SerializeField] private HealthIndicationSingleMaterial _healthIndication;
        [SerializeField] private DamageEmitParticles _damageEmitParticles;
        [SerializeField] private DeathFlashSingleMaterial _deathFlash;
        
        private WaitForSeconds _preattackDelay;  
        
        public void Initialize()
        {
            _preattackDelay = new (_preattackDuration);
            
            _spiderwebController.Initialize();
            _damageFlash.Initialize();
            _preAttackFlash.Initialize();
            _healthIndication.Initialize();
            _deathFlash.Initialize();

            _movement.PreAttackStarted += StartPreattack;
            _megaspider.Started += OnMegaspiderStarted;
            _megaspider.Damaged += _damageFlash.Play;
            _megaspider.HealthChanged += _healthIndication.Refresh;
            _megaspider.HealthChanged += _damageEmitParticles.HandleHealthChanged;
            _megaspider.Dead += _deathFlash.Play;
            _megaspider.Dead += _damageEmitParticles.HandleDead;
            _movement.DeathStateEnded += _damageEmitParticles.HandleDeathEnd;
        }

        private void OnDestroy()
        {
            _movement.PreAttackStarted -= StartPreattack;
            _megaspider.Started -= OnMegaspiderStarted;
            _megaspider.Damaged -= _damageFlash.Play;
            _megaspider.HealthChanged -= _healthIndication.Refresh;
            _megaspider.HealthChanged -= _damageEmitParticles.HandleHealthChanged;
            _megaspider.Dead -= _deathFlash.Play;
            _megaspider.Dead -= _damageEmitParticles.HandleDead;
            _movement.DeathStateEnded -= _damageEmitParticles.HandleDeathEnd;
        }

        private void StartPreattack()
        {
            _preAttackFlash.PreAttackStart();
            StartCoroutine(StopPreattackDelay());
        }

        private IEnumerator StopPreattackDelay()
        {
            yield return _preattackDelay;
            _preAttackFlash.PreAttackEnd();
        }

        private void OnMegaspiderStarted()
        {
            _healthIndication.Reset();
            _damageEmitParticles.Initialize();
            _deathFlash.Initialize();
        }
    }
}
