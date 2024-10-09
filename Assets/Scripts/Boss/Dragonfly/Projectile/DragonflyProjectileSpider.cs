using System;
using UnityEngine;

public class DragonflyProjectileSpider : EnemyBase
{
    [SerializeField] private EnemyTypes _enemyType = EnemyTypes.DragonflyProjectile;
    [SerializeField] private DragonflyProjectileMovementSpider _movement;
    [SerializeField] private DragonflyProjectilePresentation _presentation;
    [SerializeField] private Collider2D _collider;
    public override EnemyTypes EnemyType => _enemyType;
    
    public event Action OnEnterAnimationEnd;
    
    
    private bool _isDead = false;
    

    private void OnEnable()
    {
        LampAttackModel.OnLampAttack += TMPHandleLampAttack;
        _movement.OnEnterAnimationEnd += OnEnterAnimationEndHandle;
        _movement.OnFallEnded += OnFallEndedHandle;
        
    }
    
    private void OnDisable()
    {
        LampAttackModel.OnLampAttack -= TMPHandleLampAttack;
        _movement.OnEnterAnimationEnd -= OnEnterAnimationEndHandle;
        _movement.OnFallEnded -= OnFallEndedHandle;
    }

    private void OnFallEndedHandle()
    {
        gameObject.SetActive(false);
    }

    private void OnEnterAnimationEndHandle()
    {
        OnEnterAnimationEnd?.Invoke();
    }


    public void Initialize(int direction)
    {
        _movement.Initialize(direction);
        _presentation.Initialize();
        _isDead = false;
        ReadyToLampDamage = false;
        _collider.enabled = false;
        // Presentation setup
    }

    public override void AttackStart()
    {
        ReadyToCollide = true;
        ReceivedLampAttack = false;
        _movement.TriggerAttack();
        _collider.enabled = true;
    }

    public override void HandleEnteringAttackZone(Collider2D collider)
    {
        ReadyToLampDamage = true;
        Debug.Log("Dragonfly Projectile: Entered attack zone");
    }

    public override void HandleCollisionWithLamp()
    {
        Debug.Log("Collided with lamp");
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
        ReceivedLampAttack = true;
        _isDead = true;
        _collider.enabled = false;
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
}
