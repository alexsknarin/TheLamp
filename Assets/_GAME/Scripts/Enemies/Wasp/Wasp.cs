using System;
using _GAME.Scripts.Lib.Enums;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;
using UnityEngine.Serialization;

namespace _GAME.Scripts.Enemies.Wasp
{
    public class Wasp: CollidableEnemy, IBoss, IAnimatedEnemy
    {
        [Header("-- Attributes --")]
        [SerializeField] private int _maxHealth = 24;
        [SerializeField] private int _currentHealth;
        [SerializeField] private float _collisionRadius = 0.22f;
        [Header("-- Movement --")]
        [SerializeField] private WaspMovement _movement;
        [FormerlySerializedAs("_animationClipEventsListener")] [FormerlySerializedAs("_animationEventsListener")] [SerializeField] private WaspAnimationClipEventListener _animationClipEventListener;
    
        public event Action SpreadRequested;
        public event Action<CollidableEnemy> AnimatedAttackStarted;
        public event Action Damaged; // TODO: Remove this event and check if other classes need it
        public event Action<int, int> HealthChanged;
        public event Action Dead;
    
        public override Vector2 Position => _movement.Position;
        public override float Radius => _collisionRadius;
        public Transform MovementTransform => _movement.transform;
        public override void Initialize()
        {
            _movement.Initialize();
            _movement.SetCollisionRadius(_collisionRadius);
            _animationClipEventListener.ClipEnded += OnClipEnded;
            _animationClipEventListener.SpreadTgiggered += OnSpreadTriggered;
            _animationClipEventListener.AttackStarted += OnAttackStateStarted;
            _movement.DeathStateEnded += OnDeathStateEnded;
        }

        private void OnDestroy()
        {
            _animationClipEventListener.ClipEnded -= OnClipEnded;
            _animationClipEventListener.SpreadTgiggered -= OnSpreadTriggered;
            _animationClipEventListener.AttackStarted -= OnAttackStateStarted;
            _movement.DeathStateEnded -= OnDeathStateEnded;
        }

        public override void Play()
        {
            _movement.Play();
            CollisionState = CollidableState.Outside;
            _currentHealth = _maxHealth;
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
                _movement.SetDead();
            }
            else
            {
                Debug.Log($"Damage Received: {damageAmount}.");
                Damaged?.Invoke();
                HealthChanged?.Invoke(_currentHealth, _maxHealth);
            }
        }

        public override void Attack()
        {
            IsReceivedLampAttackDamage = false;
        }

        public override void DoDeath()
        {
            _movement.SetDead();
        }
    
        public override Vector3 ProvideImpactPoint()
        {
            Vector3 position = _movement.Position;
            position.z = 0;
            return position;
        }

        public override void HandleCollision()
        {
            _movement.SetCollidedWithLamp();
            CollisionState = CollidableState.AfterCollision;
        }
    
        // Calls from animation clips
        private void OnClipEnded()
        {
            _movement.ClipEnded();
        }
    
        private void OnSpreadTriggered()
        {
            SpreadRequested?.Invoke();
        }
    
        private void OnAttackStateStarted()
        {
            IsReceivedLampAttackDamage = false;
            AnimatedAttackStarted?.Invoke(this);
        }
    
    
    
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(_movement.Position, _collisionRadius);
        }
    }
}
