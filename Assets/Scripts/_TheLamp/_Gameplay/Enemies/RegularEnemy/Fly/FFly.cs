using System;
using UnityEngine;
using UnityEngine.Pool;

public class FFly : FEnemy
{
    [Header("-- Attributes --")]
    [SerializeField] private int _maxHealth = 1;
    [SerializeField] private int _currentHealth;
    [SerializeField] private float _collisionRadius = 0.1f;
    [Header("-- Movement --")]
    [SerializeField] private FFlyMovement _movement;
    
    public event Action Started;
    public event Action Damaged;
    public event Action<int, int> HealthChanged; 
    public event Action Dead;
    public override float Radius => _collisionRadius;
    public override Vector2 Position => _movement.Position2D;
    public override bool IsReadyToAttack => CheckIsReadyToAttack();
    
    public override void Initialize()
    {
        _movement.Initialize();
        _movement.ReadyToAttackStateStarted += OnReadyToAttackStateStarted;
        _movement.ReadyToAttackStateEnded += OnReadyToAttackStateEnded;
        _movement.DeathStateEnded += OnDeathStateEnded;
    }

    private void OnDestroy()
    {
        _movement.ReadyToAttackStateStarted -= OnReadyToAttackStateStarted;
        _movement.ReadyToAttackStateEnded -= OnReadyToAttackStateEnded;
        _movement.DeathStateEnded -= OnDeathStateEnded;
    }

    public override void Play()
    {
        _currentHealth = _maxHealth;
        _isInAttackReadyMovementState = false;
        IsReadyForDamage = false;
        IsReceivedAttack = false;
        CollisionState = CollidableState.Outside;
        Started?.Invoke();
        HealthChanged?.Invoke(_currentHealth, _maxHealth);
        _movement.Play();
    }

    public override void ReceiveDamage(int damageAmount)
    {
        IsReceivedAttack = true;
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
            float x = _movement.Position2D.x;
            float y = _movement.Position2D.y;
            if (_movement.SideDirection < 0)
            {
                if ((x < 0 && y < 0.30f) || (x > 0 && y < 1.65f))
                {
                    return true; 
                }
            }
            if (_movement.SideDirection > 0)
            {
                if ((x > 0 && y < 0.30f) || (x < 0 && y < 1.65f))
                {
                    return true; 
                }
            }
        }
        return false;
    }

    public override void Attack()
    {
        _isInAttackReadyMovementState = false;
        IsReceivedAttack = false;
        _movement.TriggerAttack();
    }

    public override void Spread()
    {
        _movement.TriggerSpread();
    }

    public override void DoDeath()
    {
        _movement.TriggerDeath();
    }

    public override void HandleCollision()
    {
        IsCollided = true;
        CollisionState = CollidableState.AfterCollision;
        _movement.TriggerFall();
    }

    // --- Events ---
    private void OnReadyToAttackStateStarted()
    {
        _isInAttackReadyMovementState = true;
    }

    private void OnReadyToAttackStateEnded()
    {
        _isInAttackReadyMovementState = false;
    }

    private void OnDeathStateEnded()
    {
        _objectPool.Release(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _collisionRadius);
    }
}
