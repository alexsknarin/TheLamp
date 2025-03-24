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
        [SerializeField] private LadybugDamageFlash _damageFlash;
        [SerializeField] private DeathFlash _deathFlash;
        [SerializeField] private HealthIndication _healthIndication;
        [SerializeField] private TrailResetHandler _trailResetHandler;

        public void Initialize()
        {
            _preAttackFlash.Initialize();
            _damageFlash.Initialize();
            _deathFlash.Initialize();
            _healthIndication.Initialize();
            _trailResetHandler.Initialize();
        
            _movement.PreAttackStarted += _preAttackFlash.PreAttackStart;
            _movement.PreAttackEnded += _preAttackFlash.PreAttackEnd;
            _movement.FallEnded += _trailResetHandler.Initialize;
            _megabeetle.Started += OnLadybugStarted;
            _megabeetle.Damaged += _damageFlash.Play;
            _megabeetle.HealthChanged += _healthIndication.Refresh;
            _megabeetle.Dead += _deathFlash.Play;
        }

        private void OnDestroy()
        {
            _movement.PreAttackStarted -= _preAttackFlash.PreAttackStart;
            _movement.PreAttackEnded -= _preAttackFlash.PreAttackEnd;
            _movement.FallEnded -= _trailResetHandler.Initialize;
            _megabeetle.Started -= OnLadybugStarted;
            _megabeetle.Damaged -= _damageFlash.Play;
            _megabeetle.HealthChanged -= _healthIndication.Refresh;
            _megabeetle.Dead -= _deathFlash.Play;
        }

        private void OnLadybugStarted()
        {
            _trailResetHandler.Initialize();
            _deathFlash.Initialize();
        }
    }
}
