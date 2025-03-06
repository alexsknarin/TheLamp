using System;
using UnityEngine;

public class FWasp: CollidableEnemy, IBoss, IAnimatedEnemy
{
    [Header("-- Attributes --")]
    [SerializeField] private int _maxHealth = 24;
    [SerializeField] private int _currentHealth;
    [SerializeField] private float _collisionRadius = 0.175f;
    [Header("-- Movement --")]
    [SerializeField] private FWaspMovement _movement;
    [SerializeField] private WaspAnimationEventsListener _animationEventsListener;
    
    public event Action SpreadRequested;
    public event Action<CollidableEnemy> AnimatedAttackStarted;
    
    public override Vector2 Position => _movement.Position;
    public override float Radius => _collisionRadius;
    public override void Initialize()
    {
        _movement.Initialize();
        _animationEventsListener.ClipEnded += OnClipEnded;
        _animationEventsListener.SpreadTgiggered += OnSpreadTriggered;
        _animationEventsListener.AttackStarted += OnAttackStateStarted;

    }

    private void OnDestroy()
    {
        _animationEventsListener.ClipEnded -= OnClipEnded;
        _animationEventsListener.SpreadTgiggered -= OnSpreadTriggered;
        _animationEventsListener.AttackStarted -= OnAttackStateStarted;
    }

    public override void Play()
    {
        _movement.Play();
        CollisionState = CollidableState.Outside;
    }

    public override void ReceiveDamage(int damageAmount)
    {
        throw new NotImplementedException();
    }

    public override void Attack()
    {
        throw new NotImplementedException();
    }

    public override void DoDeath()
    {
        throw new NotImplementedException();
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
        AnimatedAttackStarted?.Invoke(this);
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(_movement.Position, _collisionRadius);
    }
}
