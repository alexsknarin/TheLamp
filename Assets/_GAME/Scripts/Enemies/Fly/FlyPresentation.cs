using _GAME.Scripts.Enemies.Generic.Presentation;
using _GAME.Scripts.Lib;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Fly
{
    public class FlyPresentation : MonoBehaviour
    {
        [SerializeField] private Fly _fly;
        [SerializeField] private FlyMovement _movement;
        [SerializeField] private PreAttackFlash _preAttackFlash;
        [SerializeField] private DamageFlash _damageFlash;
        [SerializeField] private DeathFlash _deathFlash;
        [SerializeField] private HealthIndication _healthIndication;
        [SerializeField] private TrailResetHandler _trailResetHandler;
        [SerializeField] private FlyBodyRotationHandler _flyBodyRotationHandler;
    
        public void Initialize()
        {
            _preAttackFlash.Initialize();
            _damageFlash.Initialize();
            _deathFlash.Initialize();
            _healthIndication.Initialize();
            _flyBodyRotationHandler.Initialize();
        
            _movement.PreAttackStarted += OnPreAttackStarted;
            _movement.PreAttackEnded += OnPreAttackEnded;
            _movement.SpreadStateEnded += _trailResetHandler.Initialize;
            _fly.Started += OnFlyStarted;
            _fly.Damaged += OnFlyDamaged;
            _fly.HealthChanged += _healthIndication.Refresh;
            _fly.Dead += OnFlyDead;
        
        }

        private void OnDestroy()
        {
            _movement.PreAttackStarted -= OnPreAttackStarted;
            _movement.PreAttackEnded -= OnPreAttackEnded;
            _movement.SpreadStateEnded -= _trailResetHandler.Initialize;
            _fly.Started -= OnFlyStarted;
            _fly.Damaged -= OnFlyDamaged;
            _fly.HealthChanged -= _healthIndication.Refresh;
            _fly.Dead -= OnFlyDead;
        }

        private void OnFlyStarted()
        {
            _trailResetHandler.Initialize();
            _deathFlash.Initialize();
            _flyBodyRotationHandler.Reset();
        }

        private void OnPreAttackStarted()
        {
            _preAttackFlash.PreAttackStart();
        }

        private void OnPreAttackEnded()
        {
            _preAttackFlash.PreAttackEnd();
        }

        private void OnFlyDamaged()
        {
            _damageFlash.Play();
        }

        private void OnFlyDead()
        {
            _deathFlash.Play();
        }
    }
}
