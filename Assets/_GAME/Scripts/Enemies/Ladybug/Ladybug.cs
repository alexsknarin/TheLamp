using System;
using _GAME.Scripts.Lib.Enums;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Ladybug
{
    public class Ladybug : Enemy, IStickableWithLamp, ISpreadable
    {
        [SerializeField] private int _maxHealth = 7;
        [SerializeField] private int _currentHealth;
        [SerializeField] private float _collisionRadius = 0.125f;
        [Header("-- Movement --")]
        [SerializeField] private LadybugMovement _movement;
        [SerializeField] StickableState _stickableState;
        [SerializeField] private bool _isInStickyState;
        
        public event Action Started;
        public event Action Damaged;
        public event Action<int, int> HealthChanged; 
        public event Action Dead;
        public event Action<IStickableWithLamp> StickReadyStarted;
    
        public override bool IsReadyToAttack => CheckIsReadyToAttack();
        public bool IsSticked { get; private set; }
        public AttackBlockerState AttackBlockState { get; private set; }
        public Vector2 Position => GetCurrentPosition();
        public float Radius => _collisionRadius;
        public StickableState StickState { get; private set; }

        public override void Initialize()
        {
            _movement.SetCollisionRadius(_collisionRadius);
            _movement.Initialize();
            _movement.EnteredAttackRange += OnEnteredAttackRange;
            _movement.PreAttackStarted += OnPreAttackStarted;
            _movement.DeathStateEnded += OnDeathStateEnded;
            _movement.SpreadStateEnded += OnSpreadStateEnded;
            _movement.StickStarted += OnStickStarted;
            _movement.StickEnded += OnStickEnded;
        }

        private void OnDestroy()
        {
            _movement.EnteredAttackRange -= OnEnteredAttackRange;
            _movement.PreAttackStarted += OnPreAttackStarted;
            _movement.DeathStateEnded -= OnDeathStateEnded;
            _movement.SpreadStateEnded -= OnSpreadStateEnded;
            _movement.StickStarted -= OnStickStarted;
            _movement.StickEnded -= OnStickEnded;
        }

        private void OnStickStarted()
        {
            _isInStickyState = true;
        }

        private void OnStickEnded()
        {
            _isInStickyState = false;
        }

        public override void Play()
        {
            IsGameOver = false;
            IsDead = false;
            IsReadyForDamage = false;
            IsReceivedLampAttackDamage = false;
            _isInStickyState = false;
            _currentHealth = _maxHealth;
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
            _isInAttackReadyMovementState = false;
            IsReceivedLampAttackDamage = false;
            _movement.TriggerAttack();
        }

        public void Spread()
        {
            if (!_isInStickyState) _movement.TriggerSpread();
        }

        public override void DoDeath()
        {
            _movement.TriggerDeath();
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(transform.position, _collisionRadius);
        }


        // Handle sticky stuff

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
        
        }

        public void HandleExitAttackZone()
        {
            IsSticked = false;
            StickState = StickableState.Outside;
            IsReadyForDamage = false;
            // Most likely dead at this moment
        }

        public void HandleEnterAttackBlockerZone()
        {
            AttackBlockState = AttackBlockerState.Inside;
        }

        public void HandleExitAttackBlockerZone()
        {
            AttackBlockState = AttackBlockerState.Outisde;
        }

        public void HandleLampDestroyed()
        {
            if (IsSticked)
            {
                _movement.TriggerFall();    
            }
            else
            {
                _movement.TriggerSpread();
            }
        }

        public Vector3 ProvideImpactPoint()
        {
            return transform.position;
        }

        private void OnEnteredAttackRange()
        {
            StickReadyStarted?.Invoke(this);
        }

        private void OnPreAttackStarted()
        {
            StickReadyStarted?.Invoke(this);
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
        
        private Vector2 GetCurrentPosition()
        {
            if (_isInStickyState)
            {
                return transform.position;
            }
            return _movement.Position2D;
        }
    }
}
