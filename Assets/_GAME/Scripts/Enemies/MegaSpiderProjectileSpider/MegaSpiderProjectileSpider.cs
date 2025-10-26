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

        public event Action Started;
        public event Action JumpStarted;
        public event Action FallStarted;
        public event Action Damaged;
        public event Action FallEnded;
        
        public bool IsAttackStarted => _isAttackStarted;
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
            IsReceivedLampAttackDamage = false;
            _isAttackStarted = false;
            Started?.Invoke();
        }

        public override void ReceiveDamage(int damageAmount)
        {
            if (damageAmount < 1f) return;
            IsReceivedLampAttackDamage = true;
            _movement.TriggerFall(AttackResult.Fail);
            Damaged?.Invoke();
        }

        public override void Attack()
        {
            _movement.TriggerAttack();
            _isAttackStarted = true;
            JumpStarted?.Invoke();
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
            FallStarted?.Invoke();
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
