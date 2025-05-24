using _GAME.Scripts.Enemies.Generic.Presentation;
using _GAME.Scripts.Lib;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Mothling
{
    public class MothlingPresentation : MonoBehaviour, IInitializable
    {
        [SerializeField] private Mothling _mothling;
        [SerializeField] private MothlingMovement _movement;
        [SerializeField] private PreAttackFlash _preAttackFlash;
        [SerializeField] private DamageFlash _damageFlash;
        [SerializeField] private DeathFlash _deathFlash;
        [SerializeField] private TrailResetHandler _trailResetHandler;
        [SerializeField] private MothlingBodyRotationHandler _mothlingBodyRotationHandler;
    
        public void Initialize()
        {
            _preAttackFlash.Initialize();
            _damageFlash.Initialize();
            _deathFlash.Initialize();
            _mothlingBodyRotationHandler.Initialize();
        
            _movement.PreAttackStarted += OnPreAttackStarted;
            _movement.PreAttackEnded += OnPreAttackEnded;
            _movement.SpreadStateEnded += _trailResetHandler.Initialize;
            _mothling.Started += OnMothlingStarted;
            _mothling.Damaged += OnMothlingDamaged;
            _mothling.Dead += OnMothlingDead;
        
        }

        private void OnDestroy()
        {
            _movement.PreAttackStarted -= OnPreAttackStarted;
            _movement.PreAttackEnded -= OnPreAttackEnded;
            _movement.SpreadStateEnded -= _trailResetHandler.Initialize;
            _mothling.Started -= OnMothlingStarted;
            _mothling.Damaged -= OnMothlingDamaged;
            _mothling.Dead -= OnMothlingDead;
        }

        private void OnMothlingStarted()
        {
            _trailResetHandler.Initialize();
            _deathFlash.Initialize();
            _mothlingBodyRotationHandler.Reset();
        }

        private void OnPreAttackStarted()
        {
            _preAttackFlash.PreAttackStart();
        }

        private void OnPreAttackEnded()
        {
            _preAttackFlash.PreAttackEnd();
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
