using System;
using _GAME.Scripts.Lib.Enums;
using UnityEngine;

namespace _GAME.Scripts.Enemies.DragonflyProjectileSpider
{
    public class DragonflyProjectileSpider : CollidableEnemy
    {
        [SerializeField] private float _collisionRadius = 0.15f;
        [SerializeField] private DragonflyProjectileSpiderMovement _movement;
        [SerializeField] private DragonflySpiderPresentation _presentation;
        [SerializeField] private TrailRenderer _trailRenderer;
        private int _direction;
        public override Vector2 Position => transform.position;
        public event Action EnterAnimationEnded;
        public event Action<Enemy, bool> Deactivated;
    
        public override void Initialize()
        {
            _movement.EnterAnimationEnded += OnEnterAnimationEndHandle;
            _movement.FallEnded += OnFallEndedHandle;
            
            Reset();
        }

        private void OnDestroy()
        {
            _movement.EnterAnimationEnded -= OnEnterAnimationEndHandle;
            _movement.FallEnded -= OnFallEndedHandle;
        }

        public void Reset()
        {
            _presentation.Initialize();
            Radius = _collisionRadius;
            gameObject.SetActive(false);
        }

        public void SetDirection(int direction)
        {
            _direction = direction;
        }

        public override void Play()
        {
            IsGameOver = false;
            IsReadyForDamage = false;
            IsReceivedLampAttackDamage = false;
            gameObject.SetActive(true);
            _trailRenderer.Clear();
            _trailRenderer.emitting = false;
            _movement.Play(_direction);
            _presentation.Play();
        }

        public void StartPreAttack()
        {
            _presentation.PreAttackStart();
        }

        public override void Attack()
        {
            // ReceivedLampAttack = false;
            _movement.TriggerAttack();
            _presentation.PreAttackEnd();
            _trailRenderer.emitting = true;
        }

        public override void HandleCollision()
        {
            // ReadyToCollide = false;
            CollisionState = CollidableState.AfterCollision;
            IsReadyForDamage = true;
            _movement.TriggerFall();
        }

        public override void ReceiveDamage(int damage)
        {
            if (damage < 1f) return;
            _movement.TriggerFall();
            _presentation.DeathFlash();
            IsReceivedLampAttackDamage = true;
        }

        public override void DoDeath()
        {
            throw new NotImplementedException();
        }

        public override Vector3 ProvideImpactPoint()
        {
            return transform.position;
        }
   
        // Event Handlers
        private void OnEnterAnimationEndHandle()
        {
            _presentation.SwitchToCaughtState();
            EnterAnimationEnded?.Invoke();
        }

        private void OnFallEndedHandle()
        {
            gameObject.SetActive(false);
            Deactivated?.Invoke(this, IsReceivedLampAttackDamage);
        }
    
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _collisionRadius);
        }
    }
}
