using System;
using UnityEngine;

public class DragonflyProjectileSpider : CollidableEnemy
{
    [SerializeField] private float _collisionRadius = 0.15f;
    [SerializeField] private EnemyType _enemyType = EnemyType.DragonflyProjectile;
    [SerializeField] private DragonflyProjectileMovementSpider _movement;
    // [SerializeField] private DragonflySpiderPresentation _presentation;
    [SerializeField] private TrailRenderer _trailRenderer;
    private int _direction;
    public override Vector2 Position => transform.position;
    public event Action EnterAnimationEnded;


    private void OnEnable()
    {
        // LampAttackModel.OnLampAttackEvent += TMPHandleLampAttack; // TODO: fix this
        _movement.EnterAnimationEnded += OnEnterAnimationEndHandle;
        _movement.FallEnded += OnFallEndedHandle;
        
    }
    private void OnDisable()
    {
        // LampAttackModel.OnLampAttackEvent -= TMPHandleLampAttack;
        _movement.EnterAnimationEnded -= OnEnterAnimationEndHandle;
        _movement.FallEnded -= OnFallEndedHandle;
    }

    public override void Initialize()
    {
        // _presentation.Initialize();
        Radius = _collisionRadius;
    }
    
    public void SetDirection(int direction)
    {
        _direction = direction;
    }

    public override void Play()
    {
        _trailRenderer.Clear();
        _trailRenderer.emitting = false;
        _movement.Play(_direction);
        // _presentation.Play();
        IsReadyForDamage = false;
    }

    public void StartPreAttack()
    {
        // _presentation.PreAttackStart();
    }

    public override void Attack()
    {
        // ReceivedLampAttack = false;
        _movement.TriggerAttack();
        // _presentation.PreAttackEnd();
        _trailRenderer.emitting = true;
    }

    public override void HandleCollision()
    {
        // ReadyToCollide = false;
        IsReadyForDamage = true;
        _movement.TriggerFall();
    }

    public override void ReceiveDamage(int damage)
    {
        if (damage < 1f) return;
        
        // ReceivedLampAttack = true;
        _movement.TriggerFall();
        // _presentation.DeathFlash();
        // Presentation - show damage effect    
    }

    public override void DoDeath()
    {
        throw new NotImplementedException();
    }

    public override Vector3 ProvideImpactPoint()
    {
        return transform.position;
    }
   
    // Event Handlers
    private void OnEnterAnimationEndHandle()
    {
        // _presentation.SwitchToCaughtState();
        EnterAnimationEnded?.Invoke();
    }

    private void OnFallEndedHandle()
    {
        gameObject.SetActive(false);
    }
}
