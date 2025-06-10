using _GAME.Scripts.Enemies.Generic.Presentation;
using _GAME.Scripts.Lib;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Moth
{
    public class MothPresentation : MonoBehaviour
    {
        private static readonly int StartFall = Animator.StringToHash("StartFall");
        private static readonly int EndFall = Animator.StringToHash("EndFall");
        [SerializeField] private Moth _moth;
        [SerializeField] private MothMovement _movement;
        [SerializeField] private PreAttackFlash _preAttackFlash;
        [SerializeField] private DamageFlash _damageFlash;
        [SerializeField] private DeathFlash _deathFlash;
        [SerializeField] private HealthIndication _healthIndication;
        [SerializeField] private TrailResetHandler _trailResetHandler;
        [SerializeField] private MothBodyRotationHandler _mothBodyRotationHandler;
        [Header("Animation")]
        [SerializeField] private Animator _wingsAnimator;
    
        public void Initialize()
        {
            _preAttackFlash.Initialize();
            _damageFlash.Initialize();
            _deathFlash.Initialize();
            _healthIndication.Initialize();
            _mothBodyRotationHandler.Initialize();
        
            _movement.PreAttackStarted += OnPreAttackStarted;
            _movement.PreAttackEnded += OnPreAttackEnded;
            _movement.SpreadStateEnded += _trailResetHandler.Initialize;
            
            _movement.FallStateStarted += OnFallStateStarted;
            _movement.DeathStateStarted += OnFallStateStarted;
            _movement.FallStateEnded += OnFallStateEnded;
            _movement.DeathStateEnded += OnFallStateEnded;
            
            _moth.Started += OnFlyStarted;
            _moth.Damaged += OnFlyDamaged;
            _moth.HealthChanged += _healthIndication.Refresh;
            _moth.Dead += OnFlyDead;
        
        }

        private void OnDestroy()
        {
            _movement.PreAttackStarted -= OnPreAttackStarted;
            _movement.PreAttackEnded -= OnPreAttackEnded;
            _movement.SpreadStateEnded -= _trailResetHandler.Initialize;
            
            _movement.FallStateStarted += OnFallStateStarted;
            _movement.DeathStateStarted += OnFallStateStarted;
            _movement.FallStateEnded += OnFallStateEnded;
            _movement.DeathStateEnded += OnFallStateEnded;
            
            _moth.Started -= OnFlyStarted;
            _moth.Damaged -= OnFlyDamaged;
            _moth.HealthChanged -= _healthIndication.Refresh;
            _moth.Dead -= OnFlyDead;
        }

        private void OnFlyStarted()
        {
            _trailResetHandler.Initialize();
            _deathFlash.Initialize();
        }

        private void OnPreAttackStarted()
        {
            _preAttackFlash.PreAttackStart();
        }

        private void OnPreAttackEnded()
        {
            _preAttackFlash.PreAttackEnd();
        }

        private void OnFallStateStarted()
        {
            _wingsAnimator.SetTrigger(StartFall);
        }

        private void OnFallStateEnded()
        {
            _wingsAnimator.SetTrigger(EndFall);
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
