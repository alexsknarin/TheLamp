using System;
using UnityEngine;

public class FMegamothling : CollidableEnemy, IBoss
{
    [Header("-- Attributes --")]
    [SerializeField] private int _maxHealth = 10;
    [SerializeField] private int _currentHealth;
    [SerializeField] private float _collisionRadius = 0.175f;
    [Header("-- Movement --")]
    [SerializeField] private FMegamothlingMovement _movement;
    
    public event Action Started;
    public event Action Damaged;
    public event Action<int, int> HealthChanged;
    public event Action Dead;
    public event Action SpreadRequested;
    public override float Radius => _collisionRadius;
    public override Vector2 Position => _movement.Position2D;
    public override bool IsReadyToAttack => CheckIsReadyToAttack();
    
    public override void Initialize()
    {
        _movement.Initialize();
        _movement.ReadyToAttackStateStarted += OnReadyToAttackStateStarted;
        _movement.ReadyToAttackStateEnded += OnReadyToAttackStateEnded;
        _movement.DeathStateEnded += OnDeathStateEnded;
        _movement.PreAttackStarted += OnPreAttackStarted;
    }
    
    private void OnDestroy()
    {
        _movement.ReadyToAttackStateStarted -= OnReadyToAttackStateStarted;
        _movement.ReadyToAttackStateEnded -= OnReadyToAttackStateEnded;
        _movement.DeathStateEnded -= OnDeathStateEnded;
        _movement.PreAttackStarted -= OnPreAttackStarted;
    }

    public override void Play()
    {
        Debug.Log(" ---------------- FMegamothling: Play");
        _currentHealth = _maxHealth;
        _isInAttackReadyMovementState = false;
        IsReadyForDamage = false;
        IsReceivedLampAttackDamage = false;
        CollisionState = CollidableState.Outside;
        Started?.Invoke();
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

    public override void Attack()
    {
        _isInAttackReadyMovementState = false;
        IsReceivedLampAttackDamage = false;
        _movement.TriggerAttack();
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

    private bool CheckIsReadyToAttack()
    {
        if (_isInAttackReadyMovementState)
        {
            float x = _movement.Position2D.x;
            float y = _movement.Position2D.y;
            if ((y < 0.0f) || (Mathf.Abs(x) > 1.1f && y > 0.0f))
            {
                return true;
            }
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _collisionRadius);
    }

    private void OnPreAttackStarted()
    {
        SpreadRequested?.Invoke();
    }
}
