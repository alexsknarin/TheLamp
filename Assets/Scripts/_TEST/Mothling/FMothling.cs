using System;
using UnityEngine;

[RequireComponent(typeof(FMothlingMovement), typeof(FMothlingPresentation))]   
public class FMothling: MonoBehaviour, ICollidable, IInitializable
{
    [Header("-- Attributes --")]
    [SerializeField] private int _maxHealth = 1;
    [SerializeField] private int _currentHealth;
    [SerializeField] private float _collisionRadius = 0.075f;
    [Header("-- Movement --")]
    [SerializeField] private FMothlingMovement _movement;
    
    private bool _isInAttackReadyState = false;
    
    public float Radius => _collisionRadius;
    public Vector2 Position => transform.position;
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
        _isInAttackReadyState = false;
        _movement.Play();
    }

    private bool CheckIsReadyToAttack()
    {
        if (_isInAttackReadyState)
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
        _isInAttackReadyState = false;
        _movement.TriggerAttack();
    }

    public void Spread()
    {
        _movement.TriggerSpread();
    }

    public void HandleCollision(Vector2 newPosition)
    {
        _movement.TriggerFall(newPosition);
    }

    public void HandleDeath()
    {
        _movement.TriggerDeath();
    }

    private void OnPatrolStarted()
    {
        _isInAttackReadyState = true;
    }


    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _collisionRadius);
    }
}
