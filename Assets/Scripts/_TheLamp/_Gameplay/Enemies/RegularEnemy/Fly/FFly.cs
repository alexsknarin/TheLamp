using System;
using UnityEngine;
using UnityEngine.Pool;

public class FFly : FEnemy
{
    [Header("-- Attributes --")]
    [SerializeField] private int _maxHealth = 1;
    [SerializeField] private int _currentHealth;
    [SerializeField] private float _collisionRadius = 0.075f;
    [SerializeField] private bool _isReadyForDamage = false;
    [Header("-- Movement --")]
    [SerializeField] private FFlyMovement _movement;
    private IObjectPool<FEnemy> _objectPool;
    private bool _isInAttackReadyMovementState = false;
    
    public event Action Started;
    public event Action Damaged;
    public event Action Dead;
    public override float Radius => _collisionRadius;
    public override Vector2 Position => transform.position;
    public override bool IsReadyToAttack => CheckIsReadyToAttack();
    
    public override void Initialize()
    {
        _movement.Initialize();
        _movement.PatrolStarted += OnPatrolStarted;
        _movement.DeathStateEnded += OnDeathStateEnded;
        // TODO: release from pool on death state end
    }

    private void OnDestroy()
    {
        _movement.PatrolStarted -= OnPatrolStarted;
        _movement.DeathStateEnded -= OnDeathStateEnded;
    }
}
