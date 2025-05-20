using _GAME.Scripts.Enemies.Generic.Presentation;
using _GAME.Scripts.Lib;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megamothling
{
    public class MegamothlingPresentation : MonoBehaviour
    {
        [SerializeField] private Megamothling _megamothling;
        [SerializeField] private MegamothlingMovement _movement;
        [SerializeField] private PreAttackFlash _preAttackFlash;
        [SerializeField] private DamageFlash _damageFlash;
        [SerializeField] private DeathFlash _deathFlash;
        [SerializeField] private HealthIndication _healthIndication;
        [SerializeField] private TrailResetHandler _trailResetHandler;
        [SerializeField] private MegamothlingBodyRotationHandler _megamothlingBodyRotationHandler;
        [Header("Animation")]
        [SerializeField] private Animator _legsAnimator;
    
        public void Initialize()
        {
            _preAttackFlash.Initialize();
            _damageFlash.Initialize();
            _deathFlash.Initialize();
            _healthIndication.Initialize();
            _megamothlingBodyRotationHandler.Initialize();
        
            _movement.PreAttackStarted += OnPreAttackStarted;
            _movement.PreAttackEnded += OnPreAttackEnded;
            _movement.AttackEnded += OnAttackEnded;
            _movement.FallStateEnded += OnFallStateEnded;
            _megamothling.Started += OnMothlingStarted;
            _megamothling.Damaged += OnMothlingDamaged;
            _megamothling.Dead += OnMothlingDead;
            _megamothling.HealthChanged += _healthIndication.Refresh;
        
        }

        private void OnDestroy()
        {
            _movement.PreAttackStarted -= OnPreAttackStarted;
            _movement.PreAttackEnded -= OnPreAttackEnded;
            _movement.AttackEnded -= OnAttackEnded;
            _movement.FallStateEnded -= OnFallStateEnded;
            _megamothling.Started -= OnMothlingStarted;
            _megamothling.Damaged -= OnMothlingDamaged;
            _megamothling.Dead -= OnMothlingDead;
            _megamothling.HealthChanged -= _healthIndication.Refresh;
        }

        private void OnMothlingStarted()
        {
            _trailResetHandler.Initialize();
            _deathFlash.Initialize();
            _healthIndication.Initialize();
            _megamothlingBodyRotationHandler.Play();
        }

        private void OnPreAttackStarted()
        {
            _preAttackFlash.PreAttackStart();
        }

        private void OnPreAttackEnded()
        {
            _preAttackFlash.PreAttackEnd();
            _legsAnimator.SetTrigger("AttackStart"); // TODO: cache
        }

        private void OnAttackEnded()
        {
            _legsAnimator.SetTrigger("Fall");
        }

        private void OnFallStateEnded()
        {
            _legsAnimator.SetTrigger("Return");
        }

        private void OnMothlingDamaged()
        {
            _damageFlash.Play();
        }

        private void OnMothlingDead()
        {
            _deathFlash.Play();
        }
    }
}
