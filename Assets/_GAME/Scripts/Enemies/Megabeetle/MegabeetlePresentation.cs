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

        public void Initialize()
        {
            _preAttackFlash.Initialize();
            _damageFlash.Initialize();
            _deathFlash.Initialize();
            _damageEmitParticles.Initialize();
            _healthIndication.Initialize();
            _trailResetHandler.Initialize();
        
            _movement.PreAttackStarted += _preAttackFlash.PreAttackStart;
            _movement.PreAttackEnded += _preAttackFlash.PreAttackEnd;
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
            _movement.PreAttackStarted -= _preAttackFlash.PreAttackStart;
            _movement.PreAttackEnded -= _preAttackFlash.PreAttackEnd;
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
        }
    }
}
