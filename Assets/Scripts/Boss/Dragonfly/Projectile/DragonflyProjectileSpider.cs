using System;
using UnityEngine;
using UnityEngine.Serialization;

public class DragonflyProjectileSpider : EnemyBase
{
    [SerializeField] private EnemyType _enemyType = EnemyType.DragonflyProjectile;
    [SerializeField] private DragonflyProjectileMovementSpider _movement;
    [SerializeField] private DragonflySpiderPresentation _presentation;
    [SerializeField] private Collider2D _collider;
    [SerializeField] private TrailRenderer _trailRenderer;
    public event Action EnterAnimationEnded;
    public override EnemyType EnemyType => _enemyType;

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
        _presentation.Initialize();
    }

    public void Play(int direction)
    {
        _trailRenderer.Clear();
        _trailRenderer.emitting = false;
        _movement.Play(direction);
        _presentation.Play();
        ReadyToLampDamage = false;
        _collider.enabled = false;
        
        // Presentation setup
    }

    public void StartPreAttack()
    {
        _presentation.PreAttackStart();
    }

    public override void StartAttack()
    {
        ReadyToCollide = true;
        ReceivedLampAttack = false;
        _movement.TriggerAttack();
        _presentation.PreAttackEnd();
        _collider.enabled = true;
        _trailRenderer.emitting = true;
    }

    public override void HandleEnteringAttackZone()
    {
        ReadyToLampDamage = true;
    }

    public override void HandleCollisionWithLamp()
    {
        ReadyToCollide = false;
        ReadyToLampDamage = true;
        _movement.TriggerFall();
    }

    public override void HandleExitingAttackExitZone()
    {
        ReadyToLampDamage = false;
    }

    public override void HandleCollisionWithStickZone()
    {
        Debug.LogWarning("Spider Projectile: Lamp collision penetrated incorrectly.");
    }

    public override void ReceiveDamage(int damage)
    {
        if (damage < 1f) return;
        
        ReceivedLampAttack = true;
        _collider.enabled = false;
        OnEnemyDeathInvoke(this);
        _movement.TriggerFall();
        _presentation.DeathFlash();
        // Presentation - show damage effect    
    }

    public override void UpdateAttackAvailability()
    {
        throw new System.NotImplementedException();
    }

    public override void ReturnToPool()
    {
        throw new System.NotImplementedException();
    }

    public override Vector3 ProvideImpactPoint()
    {
        return transform.position;
    }

    public override void SpreadStart()
    {
        throw new System.NotImplementedException();
    }

    private void TMPHandleLampAttack(int arg1, float arg2, float arg3, float arg4)
    {
        if (ReadyToLampDamage)
        {
            ReceiveDamage(arg1);
        }
    }
    
    // Event Handlers
    private void OnEnterAnimationEndHandle()
    {
        _presentation.SwitchToCaughtState();
        EnterAnimationEnded?.Invoke();
    }

    private void OnFallEndedHandle()
    {
        gameObject.SetActive(false);
    }
}
