using System;
using UnityEngine;

[RequireComponent(typeof(FMothlingMovement), typeof(FMothlingPresentation))]   
public sealed class FMothling: FEnemy
{
    [Header("-- Attributes --")]
    [SerializeField] private int _maxHealth = 1;
    [SerializeField] private int _currentHealth;
    [SerializeField] private float _collisionRadius = 0.075f;
    [SerializeField] private bool _isReadyForDamage = false;
    [Header("-- Movement --")]
    [SerializeField] private FMothlingMovement _movement;
    
    private bool _isInAttackReadyMovementState = false;

    public event Action Started;
    public event Action Damaged;
    public event Action Dead;
    public override float Radius => _collisionRadius;
    public override Vector2 Position => transform.position;
    // public override bool IsCollided { get; protected set; } // ????
    // public override CollidableState CollisionState { get; protected set; }
    // public override bool IsReadyForDamage { get; protected set; }
    // public override bool IsReceivedAttack { get;  protected set; }
    public override bool IsReadyToAttack => CheckIsReadyToAttack();

    public override void Initialize()
    {
        _movement.Initialize();
        _movement.PatrolStarted += OnPatrolStarted;
    }

    private void OnDestroy()
    {
        _movement.PatrolStarted -= OnPatrolStarted;
    }

    public override void Play()
    {
        _currentHealth = _maxHealth;
        _isInAttackReadyMovementState = false;
        IsReadyForDamage = false;
        _isReadyForDamage = false;
        IsReceivedAttack = false;
        CollisionState = CollidableState.Outside;
        Started?.Invoke();
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
        }
        
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

    // Handle Collisions

    public override void HandleEnterAttackZone()
    {
        IsCollided = false;
        CollisionState = CollidableState.InAttackZone;
        IsReadyForDamage = true;
        _isReadyForDamage = true;
    }

    public override void HandleCollision()
    {
        IsCollided = true;
        CollisionState = CollidableState.AfterCollision;
        _movement.TriggerFall();
    }

    public override void HandleExitAttackZone()
    {
        CollisionState = CollidableState.Outside;
        IsCollided = false;
        IsReadyForDamage = false;
        _isReadyForDamage = false;
    }

    public override Vector3 ProvideImpactPoint()
    {
        return transform.position;
    }
    
    // --- Events ---
    private void OnPatrolStarted()
    {
        _isInAttackReadyMovementState = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _collisionRadius);
    }
}
