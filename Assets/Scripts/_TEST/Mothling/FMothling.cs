using System;
using UnityEngine;

[RequireComponent(typeof(FMothlingMovement), typeof(FMothlingPresentation))]   
public class FMothling: MonoBehaviour, ICollidableWithLamp, IInitializable
{
    [Header("-- Attributes --")]
    [SerializeField] private int _maxHealth = 1;
    [SerializeField] private int _currentHealth;
    [SerializeField] private float _collisionRadius = 0.075f;
    [SerializeField] private bool _isReadyForDamage = false;
    [Header("-- Movement --")]
    [SerializeField] private FMothlingMovement _movement;
    
    private bool _isInAttackReadyMovementState = false;
    
    public float Radius => _collisionRadius;
    public Vector2 Position => transform.position;
    
    public bool IsCollided { get; private set; } // ????
    
    public CollidableState CollisionState { get; private set; }
    public bool IsReadyForDamage { get; private set; }
    
    public bool IsReadyToAttack => CheckIsReadyToAttack();

    public void Initialize()
    {
        _movement.Initialize();
        _movement.PatrolStarted += OnPatrolStarted;
    }

    private void OnDestroy()
    {
        _movement.PatrolStarted -= OnPatrolStarted;
    }

    public void Play()
    {
        _currentHealth = _maxHealth;
        _isInAttackReadyMovementState = false;
        IsReadyForDamage = false;
        _isReadyForDamage = false;
        CollisionState = CollidableState.Outside;
        _movement.Play();
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

    public void Attack()
    {
        _isInAttackReadyMovementState = false;
        _movement.TriggerAttack();
    }

    public void Spread()
    {
        _movement.TriggerSpread();
    }

    public void DoDeath()
    {
        _movement.TriggerDeath();
    }

    private void OnPatrolStarted()
    {
        _isInAttackReadyMovementState = true;
    }

    // Handle Collisions
    public void HandleEnterAttackZone()
    {
        IsCollided = false;
        CollisionState = CollidableState.InAttackZone;
        IsReadyForDamage = true;
        _isReadyForDamage = true;
    }

    public void HandleCollision()
    {
        IsCollided = true;
        CollisionState = CollidableState.AfterCollision;
        _movement.TriggerFall();
    }

    public void HandleExitAttackZone()
    {
        CollisionState = CollidableState.Outside;
        IsCollided = false;
        IsReadyForDamage = false;
        _isReadyForDamage = false;
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _collisionRadius);
    }
}
