using System;
using _GAME.Scripts.Lib.Enums;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Moth
{
    public class Moth : CollidableEnemy, ISpreadable
    {
        [Header("-- Attributes --")]
        [SerializeField] private int _maxHealth = 2;
        [SerializeField] private int _currentHealth;
        [SerializeField] private float _collisionRadius = 0.1f;
        [Header("-- Movement --")]
        [SerializeField] private MothMovement _movement;
    
        public event Action Started;
        public event Action Damaged;
        public event Action<int, int> HealthChanged; 
        public event Action Dead;
        public override float Radius => _collisionRadius;
        public override Vector2 Position => _movement.Position2D;
        public override bool IsReadyToAttack => CheckIsReadyToAttack();
    
        public override void Initialize()
        {
            _movement.SetCollisionRadius(_collisionRadius);
            _movement.Initialize();
            _movement.ReadyToAttackStateStarted += OnReadyToAttackStateStarted;
            _movement.ReadyToAttackStateEnded += OnReadyToAttackStateEnded;
            _movement.DeathStateEnded += OnDeathStateEnded;
            _movement.SpreadStateEnded += OnSpreadStateEnded;
        }

        private void OnDestroy()
        {
            _movement.ReadyToAttackStateStarted -= OnReadyToAttackStateStarted;
            _movement.ReadyToAttackStateEnded -= OnReadyToAttackStateEnded;
            _movement.DeathStateEnded -= OnDeathStateEnded;
            _movement.SpreadStateEnded -= OnSpreadStateEnded;
        }

        public override void Play()
        {
            IsGameOver = false;
            IsReadyForDamage = false;
            IsReceivedLampAttackDamage = false;
            _currentHealth = _maxHealth;
            _isInAttackReadyMovementState = false;
            CollisionState = CollidableState.Outside;
            Started?.Invoke();
            HealthChanged?.Invoke(_currentHealth, _maxHealth);
            _movement.Play();
        }

        public override void ReceiveDamage(int damageAmount)
        {
            IsReceivedLampAttackDamage = true;
            IsReadyForDamage = false;
            _currentHealth -= damageAmount;
        
            if (_currentHealth <= 0)
            {
                Dead?.Invoke();
                DoDeath();
            }
            else
            {
                Debug.Log($"Damage Received: {damageAmount}.");
                _movement.TriggerFall();
                Damaged?.Invoke();
                HealthChanged?.Invoke(_currentHealth, _maxHealth);
            }
        }

        private bool CheckIsReadyToAttack()
        {
            if (_isInAttackReadyMovementState)
            {
                return true;
            }
            return false;
        }

        public override void Attack()
        {
            Debug.Log("Attack Called.");
            _isInAttackReadyMovementState = false;
            IsReceivedLampAttackDamage = false;
            _movement.TriggerAttack();
        }

        public void Spread()
        {
            _movement.TriggerSpread();
        }

        public override void DoDeath()
        {
            _movement.TriggerDeath();
        }

        public override void HandleCollision()
        {
            CollisionState = CollidableState.AfterCollision;
            _movement.TriggerFall();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _collisionRadius);
        }
        
        private void OnSpreadStateEnded()
        {
            if (IsGameOver)
            {
                ReturnToPool();
            }
            else
            {
                _movement.Play();
            }
        }
    }
}
