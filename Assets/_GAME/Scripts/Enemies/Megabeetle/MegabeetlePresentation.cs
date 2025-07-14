using _GAME.Scripts.Enemies.Generic.Presentation;
using _GAME.Scripts.Lib;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megabeetle
{
    public class MegabeetlePresentation : MonoBehaviour
    {
        [SerializeField] private Megabeetle _megabeetle;
        [SerializeField] private MegabeetleMovement _movement;
        [SerializeField] private PreAttackFlash _preAttackFlash;
        [SerializeField] private MegabeetleDamageFlash _damageFlash;
        [SerializeField] private DamageEmitParticles _damageEmitParticles;
        [SerializeField] private DeathFlash _deathFlash;
        [SerializeField] private HealthIndication _healthIndication;
        [SerializeField] private TrailResetHandler _trailResetHandler;
        [SerializeField] private MegabeetleBodyRotationHandler _bodyRotationHandler;
        
        [Header("Animation")]
        [SerializeField] private Animator _animator;

        public void Initialize()
        {
            _preAttackFlash.Initialize();
            _damageFlash.Initialize();
            _deathFlash.Initialize();
            _damageEmitParticles.Initialize();
            _healthIndication.Initialize();
            _trailResetHandler.Initialize();
            _bodyRotationHandler.Initialize();
            
            _movement.PreAttackStarted += OnPreAttackStarted;
            _movement.PreAttackEnded += OnPreAttackEnded;
            _movement.LandingStarted += OnLandingStarted;
            _movement.StickStarted += OnStickStarted;
            _movement.StickyPreAttackStarted += OnStickyPreAttackStarted;
            _movement.StickyPreAttackPauseStarted += OnStickyPreAttackPauseStarted;
            _movement.StickyAttackStarted += OnStickyAttackStarted; 
            
            _movement.FallEnded += _trailResetHandler.Initialize;
            _movement.DeathStateEnded += _damageEmitParticles.HandleDeathEnd;
            _megabeetle.Started += OnLadybugStarted;
            _megabeetle.Damaged += _damageFlash.Play;
            _megabeetle.HealthChanged += _healthIndication.Refresh;
            _megabeetle.HealthChanged += _damageEmitParticles.HandleHealthChanged;
            _megabeetle.Dead += _deathFlash.Play;
            _megabeetle.Dead += _damageEmitParticles.HandleDead;
        }

        private void OnDestroy()
        {
            _movement.PreAttackStarted -= OnPreAttackStarted;
            _movement.PreAttackEnded -= OnPreAttackEnded;
            _movement.LandingStarted -= OnLandingStarted;
            _movement.StickStarted -= OnStickStarted;
            _movement.StickyPreAttackStarted -= OnStickyPreAttackStarted;
            _movement.StickyPreAttackPauseStarted -= OnStickyPreAttackPauseStarted;
            _movement.StickyAttackStarted -= OnStickyAttackStarted;
            
            _movement.FallEnded -= _trailResetHandler.Initialize;
            _movement.DeathStateEnded -= _damageEmitParticles.HandleDeathEnd;
            _megabeetle.Started -= OnLadybugStarted;
            _megabeetle.Damaged -= _damageFlash.Play;
            _megabeetle.HealthChanged -= _healthIndication.Refresh;
            _megabeetle.HealthChanged -= _damageEmitParticles.HandleHealthChanged;
            _megabeetle.Dead -= _deathFlash.Play;
            _megabeetle.Dead -= _damageEmitParticles.HandleDead;
        }

        private void OnLadybugStarted()
        {
            _trailResetHandler.Initialize();
            _deathFlash.Initialize();
            _bodyRotationHandler.Play();
            
            _animator.SetTrigger("Started");
        }

        private void OnPreAttackStarted()
        {
            _preAttackFlash.PreAttackStart();
            _animator.SetTrigger("ToPreAttack");
        }

        private void OnPreAttackEnded()
        {
            _preAttackFlash.PreAttackEnd();
            _animator.SetTrigger("ToAttack");
        }

        private void OnLandingStarted()
        {
            _animator.SetTrigger("ToLanding");
        }

        private void OnStickStarted()
        {
            _animator.SetTrigger("ToStick");
        }

        private void OnStickyPreAttackStarted()
        {
            _animator.SetTrigger("ToStickPreAttack");
        }

        private void OnStickyPreAttackPauseStarted()
        {
            _animator.SetTrigger("ToStickPreAttackPause");
        }

        private void OnStickyAttackStarted()
        {
            _animator.SetTrigger("ToStickAttack");
        }
    }
}
