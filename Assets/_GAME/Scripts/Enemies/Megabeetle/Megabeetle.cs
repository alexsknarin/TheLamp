using System;
using _GAME.Scripts.Lib.Enums;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Megabeetle
{
    public class Megabeetle : FEnemy, IStickableWithLamp, IBoss, IStickyAttacker
    {
        [SerializeField] private float _collisionRadius = 0.3f;
        [SerializeField] private int _maxHealth;
        [SerializeField] private int _currentHealth;
        [SerializeField] private int _healthToFallThreshold;
        [SerializeField] private MegabeetleMovement _movement;
        private int _currentHealthToFall;
    
        public event Action SpreadRequested;
        public event Action<IStickableWithLamp> StickReadyStarted;
        public event Action Started;
        public event Action Damaged;
        public event Action<int, int> HealthChanged; 
        public event Action Dead;
        public event Action<Vector3, bool, string> StickyAttackEnded;
    
        public Vector2 Position => _movement.Position2D;
        public float Radius => _collisionRadius;
        public bool IsSticked { get; private set; }
        public AttackBlockerState AttackBlockState { get; private set; }
        public StickableState StickState { get; private set; }
    
        public override void Initialize()
        {
            _movement.Initialize();
            _movement.SetCollisionRadius(_collisionRadius);
            _movement.EnteredAttackRange += OnEnteredAttackRange;
            _movement.DeathStateEnded += OnDeathStateEnded;
            _movement.AttackStarted += OnAttackStarted;
            _movement.StickyAttackEnded += OnStickyAttackEnded;
        }

        private void OnDestroy()
        {
            _movement.EnteredAttackRange -= OnEnteredAttackRange;
            _movement.DeathStateEnded -= OnDeathStateEnded;
            _movement.AttackStarted -= OnAttackStarted;
            _movement.StickyAttackEnded -= OnStickyAttackEnded;
        }

        public override void Play()
        {
            IsGameOver = false;
            IsDead = false;
            IsReadyForDamage = false;
            IsReceivedLampAttackDamage = false;
            _currentHealth = _maxHealth;
            _currentHealthToFall = _healthToFallThreshold;
            _isInAttackReadyMovementState = false;
            StickState = StickableState.Outside;
            AttackBlockState = AttackBlockerState.Outisde;
            HealthChanged?.Invoke(_currentHealth, _maxHealth);
            _movement.Play();
            Started?.Invoke();
        }

        public override void ReceiveDamage(int damageAmount)
        {
            IsReceivedLampAttackDamage = true;
            _currentHealth -= damageAmount;
        
            if (_currentHealth <= 0)
            {
                Dead?.Invoke();
                DoDeath();
                IsDead = true;
                IsReadyForDamage = false;
                StickState = StickableState.InAttackZoneDamaged;
                AttackBlockState = AttackBlockerState.Damaged;
            }
            else
            {
                Debug.Log($"Damage Received: {damageAmount}.");
                Damaged?.Invoke();
                HealthChanged?.Invoke(_currentHealth, _maxHealth);
            
                _currentHealthToFall -= damageAmount;
                if (_currentHealthToFall <= 0)
                {
                    _currentHealthToFall = _healthToFallThreshold;
                    _movement.TriggerFall();
                }
            }
        }

        public override void Attack()
        {
            throw new System.NotImplementedException();
        }

        public override void DoDeath()
        {
            Debug.Log("Megabeetle is dead.");
            _movement.TriggerDeath();
        }
    
        public void HandleEnterAttackZone()
        {
            IsSticked = false;
            StickState = StickableState.InAttackZone;
            IsReadyForDamage = true;
        }

        public void HandleStick(Transform lampTransform)
        {
            IsSticked = true;
            StickState = StickableState.Sticked;
            AttackBlockState = AttackBlockerState.Sticked;
            IsReadyForDamage = true;
            // Play event
        
            _movement.TriggerStick(lampTransform);
            Debug.Log(Position);
        }

        public void HandleExitAttackZone()
        {
            IsSticked = false;
            StickState = StickableState.Outside;
            IsReadyForDamage = false;
        }

        public void HandleEnterAttackBlockerZone()
        {
            AttackBlockState = AttackBlockerState.Inside;
        }

        public void HandleLampDestroyed()
        {
            _movement.TriggerFallOnLampDestroyed();
        }

        public Vector3 ProvideImpactPoint()
        {
            return transform.localPosition;
        }
    
        private void OnEnteredAttackRange()
        {
            StickReadyStarted?.Invoke(this);
        }
    
        private void OnAttackStarted()
        {
            SpreadRequested?.Invoke();
        }
    
        private void OnStickyAttackEnded()
        {
            StickyAttackEnded?.Invoke(transform.position, false, "Megabeetle");
        }
    
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _collisionRadius);
        }
    }
}
