using System;
using _GAME.Scripts.Lib.Enums;
using _GAME.Scripts.Lib.Interfaces;
using UnityEngine;

namespace _GAME.Scripts.Enemies.Ladybug
{
    public class Ladybug : FEnemy, IStickableWithLamp, ISpreadable
    {
        [SerializeField] private int _maxHealth = 7;
        [SerializeField] private int _currentHealth;
        [SerializeField] private float _collisionRadius = 0.125f;
        [Header("-- Movement --")]
        [SerializeField] private LadybugMovement _movement;
    
        public event Action Started;
        public event Action Damaged;
        public event Action<int, int> HealthChanged; 
        public event Action Dead;
        public event Action<IStickableWithLamp> StickReadyStarted;
    
        public override bool IsReadyToAttack => CheckIsReadyToAttack();
        public bool IsSticked { get; private set; }
        public AttackBlockerState AttackBlockState { get; private set; }
        public Vector2 Position => _movement.Position2D;
        public float Radius => _collisionRadius;
        public StickableState StickState { get; private set; }

        public override void Initialize()
        {
            _movement.Initialize();
            _movement.EnteredAttackRange += OnEnteredAttackRange;
            _movement.DeathStateEnded += OnDeathStateEnded;
        }
    
        private void OnDestroy()
        {
            _movement.EnteredAttackRange -= OnEnteredAttackRange;
            _movement.DeathStateEnded -= OnDeathStateEnded;
        }

        public override void Play()
        {
            IsDead = false;
            _currentHealth = _maxHealth;
            _isInAttackReadyMovementState = false;
            IsReadyForDamage = false;
            IsReceivedLampAttackDamage = false;
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
            _movement.TriggerSpread();
        }

        public override void DoDeath()
        {
            Debug.Log("Ladybug is dead.");
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
            Debug.Log(gameObject.name + " is ready to stick.");
            StickReadyStarted?.Invoke(this);
        }
    }
}
