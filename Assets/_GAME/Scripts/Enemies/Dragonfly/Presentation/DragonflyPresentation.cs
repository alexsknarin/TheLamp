using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Dragonfly.Presentation
{
    public class DragonflyPresentation : MonoBehaviour, IInitializable
    {
        [SerializeField] private Dragonfly _dragonfly;
        [SerializeField] private DragonflyMovement _dragonflyMovement;
        [SerializeField] private DragonflyDamageFlash _damageIndication;
        [SerializeField] private DragonflyHealthIndication _healthIndication;
        [SerializeField] private DragonflyDeathFlash _deathFlash;
        [SerializeField] private DragonflyPreAttackFlash _preAttackFlash;
        [SerializeField] private DragonflySwarmCallFX _swarmCallFX;
        
        public void Initialize()
        {
            _damageIndication.Initialize();
            _healthIndication.Initialize();
            _deathFlash.Initialize();
            _preAttackFlash.Initialize();
            _swarmCallFX.Initialize();
            
            _dragonfly.Started += OnDragonflyStarted;
            _dragonflyMovement.PreAttackStarted += OnPreAttackStarted;
            _dragonflyMovement.AttackStarted += OnPreAttackEnded;
            _dragonfly.Damaged += OnDamaged;
            _dragonfly.Died += OnDied;
            _dragonfly.HealthChanged += OnHealthChanged;
            _dragonfly.SwarmCalled += OnSwarmCalled;
            _dragonfly.ColliderTransformChanged += OnColliderTransformChanged;
        }

        private void OnDestroy()
        {
            _dragonfly.Started += OnDragonflyStarted;
            _dragonflyMovement.PreAttackStarted -= OnPreAttackStarted;
            _dragonflyMovement.AttackStarted -= OnPreAttackEnded;
            _dragonfly.Damaged -= OnDamaged;
            _dragonfly.Died -= OnDied;
            _dragonfly.HealthChanged -= OnHealthChanged;
            _dragonfly.SwarmCalled -= OnSwarmCalled;
            _dragonfly.ColliderTransformChanged -= OnColliderTransformChanged;
        }

        private void OnDragonflyStarted()
        {
            _damageIndication.Reset();
            _healthIndication.Reset();
            _deathFlash.Reset();
            _preAttackFlash.Reset();
            _swarmCallFX.Reset();
        }

        private void OnPreAttackStarted()
        {
            _preAttackFlash.PreAttackStart();
        }

        private void OnPreAttackEnded()
        {
            _preAttackFlash.PreAttackEnd();
        }

        private void OnColliderTransformChanged(Transform transform)
        {
            _damageIndication.SetContactCollisionTransform(transform);
        }

        private void OnDamaged()
        {
            _damageIndication.Play();
        }

        private void OnDied()
        {
            _deathFlash.Play();
        }

        private void OnHealthChanged(int currentHealth, int maxHealth) 
        {
            _healthIndication.Refresh(currentHealth, maxHealth);
        }

        private void OnSwarmCalled() 
        {
            _swarmCallFX.Play();
        }
    }
}
