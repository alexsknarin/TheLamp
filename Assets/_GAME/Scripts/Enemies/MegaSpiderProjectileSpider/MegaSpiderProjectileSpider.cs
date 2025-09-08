using System;
using _GAME.Scripts.Enemies.Dragonfly;
using _GAME.Scripts.Lib.Enums;
using UnityEngine;

namespace _GAME.Scripts.Enemies.MegaspiderProjectileSpider
{
    public class MegaspiderProjectileSpider : CollidableEnemy
    {
        [SerializeField] private MegaspiderProjectileSpiderMovement _movement;
        [SerializeField] private float _collisionRadius = 0.075f; 
        [SerializeField] private bool _isAttackStarted = false;
        
        public event Action FallEnded;
        
        public bool IsAttackStarted => _isAttackStarted;
        public bool IsDamaged { get; private set; }
        public override float Radius => _collisionRadius;
        public override Vector2 Position => transform.position;
        public override string CollidableName => gameObject.name;


        public override void Initialize()
        {
            _movement.Initialize();
            _movement.FallEnded += OnFallEnded;
        }

        private void OnDestroy()
        {
            _movement.FallEnded -= OnFallEnded;
        }

        public override void Play()
        {
            _movement.Play();
            IsDamaged = false;
            _isAttackStarted = false;
        }

        public override void ReceiveDamage(int damageAmount)
        {
            if (damageAmount < 1f) return;
            IsDamaged = true;
            _movement.TriggerFall(AttackResult.Fail);
            IsReceivedLampAttackDamage = true;
        }

        public override void Attack()
        {
            _movement.TriggerAttack();
            _isAttackStarted = true;
        }

        public override void DoDeath()
        {
            _movement.TriggerFall(AttackResult.Fail);
        }

        public override void HandleCollision()
        {
            CollisionState = CollidableState.AfterCollision;
            IsReadyForDamage = true;
            _movement.TriggerCollide();
        }
        
        public override void HandleExitAttackZone()
        {
            CollisionState = CollidableState.Outside;
            IsReadyForDamage = false;
            _movement.TriggerFall(AttackResult.Success);
        }

        private void OnFallEnded()
        {
            gameObject.SetActive(false);
            _isAttackStarted = false;
            FallEnded?.Invoke();
        }
    }
}
