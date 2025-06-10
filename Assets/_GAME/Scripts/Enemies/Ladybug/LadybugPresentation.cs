using _GAME.Scripts.Enemies.Generic.Presentation;
using _GAME.Scripts.Lib;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Ladybug
{
    public class LadybugPresentation : MonoBehaviour, IInitializable
    {
        private static readonly int Start = Animator.StringToHash("Start");
        private static readonly int DamageType = Animator.StringToHash("DamageType");
        private static readonly int Damage = Animator.StringToHash("Damage");
        private static readonly int AttackStart = Animator.StringToHash("AttackStart");
        private static readonly int AttackEnd = Animator.StringToHash("AttackEnd");
        [SerializeField] private Ladybug _ladybug;
        [SerializeField] private LadybugMovement _movement;
        [SerializeField] private PreAttackFlash _preAttackFlash;
        [SerializeField] private LadybugDamageFlash _damageFlash;
        [SerializeField] private DeathFlash _deathFlash;
        [SerializeField] private HealthIndication _healthIndication;
        [SerializeField] private TrailResetHandler _trailResetHandler;
        [SerializeField] private LadybugBodyRotationHandler _ladybugBodyRotationHandler;
        [SerializeField] private LadybugBodyTranslucenseController _ladybugBodyTranslucenseController;
        [SerializeField] private LadybugDamageNoiseRandomizer _ladybugDamageNoiseRandomizer;
        
        [Header("Animation")]
        [SerializeField] private Animator _animator;

        public void Initialize()
        {
            _preAttackFlash.Initialize();
            _damageFlash.Initialize();
            _deathFlash.Initialize();
            _healthIndication.Initialize();
            _trailResetHandler.Initialize();
            _ladybugBodyRotationHandler.Initialize();
            _ladybugBodyTranslucenseController.Initialize();
            _ladybugDamageNoiseRandomizer.Initialize();
        
            _movement.PreAttackStarted += OnPreAttackStarted;
            _movement.PreAttackEnded += OnPreAttackEnded;
            _movement.SpreadStateEnded += OnSpreadStateEnded;
            _movement.StickStarted += OnStickStarted;
            _ladybug.Started += OnLadybugStarted;
            _ladybug.Damaged += OnLadybugDamaged;
            _ladybug.HealthChanged += _healthIndication.Refresh;
            _ladybug.Dead += OnLadybugDead;
        }

        private void OnDestroy()
        {
            _movement.PreAttackStarted -= OnPreAttackStarted;
            _movement.PreAttackEnded -= OnPreAttackEnded;
            _movement.SpreadStateEnded -= OnSpreadStateEnded;
            _movement.StickStarted -= OnStickStarted;
            _ladybug.Started -= OnLadybugStarted;
            _ladybug.Damaged -= OnLadybugDamaged;
            _ladybug.HealthChanged -= _healthIndication.Refresh;
            _ladybug.Dead -= OnLadybugDead;
        }

        private void OnLadybugStarted()
        {
            _trailResetHandler.Initialize();
            _deathFlash.Initialize();
            _animator.SetTrigger(Start);
            _ladybugBodyRotationHandler.Play();
            _ladybugBodyTranslucenseController.SetRegular();
        }

        private void OnLadybugDamaged()
        {
            _damageFlash.Play();
            _animator.SetInteger(DamageType, Random.Range(0, 5));
            _animator.SetTrigger(Damage);
        }

        private void OnPreAttackStarted()
        {
            // TODO: set subscription directly to the methods in presentation and other sub classes 
            _preAttackFlash.PreAttackStart();
        }

        private void OnPreAttackEnded()
        {
            _preAttackFlash.PreAttackEnd();
            _animator.SetTrigger(AttackStart);
        }

        private void OnLadybugDead()
        {
            _deathFlash.Play();
        }

        private void OnStickStarted()
        {
            _animator.SetTrigger(AttackEnd);
            _ladybugBodyTranslucenseController.SetStick();
        }

        private void OnSpreadStateEnded()
        {
            _trailResetHandler.Initialize();
        }
    }
}
