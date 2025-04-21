using System;
using _GAME.Scripts.Lib.Enums;
using UnityEngine;

namespace _GAME.Scripts.Enemies.DragonflyProjectileMoth
{
    public class DragonflyProjectileMoth : CollidableEnemy
    {
        [SerializeField] private float _collisionRadius = 0.07f;
        [SerializeField] private DragonflyProjectileMovementMoth _movement;
        [SerializeField] private DragonflyMothPresentation _presentation;
    
        public override Vector2 Position => transform.position;
        public event Action<Enemy, bool> Deactivated;
    
        public override void Initialize()
        {
            _movement.FallEnded += OnFallEnded;
            Reset();
        }

        private void OnDestroy()
        {
            _movement.FallEnded -= OnFallEnded;
        }

        public void Reset()
        {
            _presentation.Initialize();
            IsReadyForDamage = false;
            IsReceivedLampAttackDamage = false;
        }

        public void SetStartPosition(Vector3 startPosition)
        {
            _movement.Initialize(startPosition);
        }

        public override void Play()
        {
            IsGameOver = false;
            IsReadyForDamage = false;
            IsReceivedLampAttackDamage = false;
            _presentation.Initialize();
            _movement.TriggerAttack();
        }

        public override void Attack() { }

        public override void DoDeath() { }

        public override void HandleCollision()
        {
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

        public override Vector3 ProvideImpactPoint()
        {
            return transform.position;
        }

        public void TriggerGameOver()
        {
            _movement.TriggerGameOver();
        }

        private void OnFallEnded()
        {
            Deactivated?.Invoke(this, IsReceivedLampAttackDamage);
            gameObject.SetActive(false);
        }
    
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _collisionRadius);
        }
    }
}
