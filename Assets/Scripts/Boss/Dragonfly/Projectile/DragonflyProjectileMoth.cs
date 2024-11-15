using UnityEngine;
using UnityEngine.Serialization;

public class DragonflyProjectileMoth : EnemyBase
{
    [FormerlySerializedAs("_enemyType")] [SerializeField] private EnemyType _enemyType = global::EnemyType.DragonflyProjectile;
    [SerializeField] private DragonflyProjectileMovementMoth _movement;
    [SerializeField] private DragonflyMothPresentation _presentation;
    public override EnemyType EnemyType => _enemyType;

    private void OnEnable()
    {
        LampAttackModel.OnLampAttackEvent += TMPHandleLampAttack;
        _movement.OnFallEndedEvent += OnFallEndedHandle;
    }

    private void OnDisable()
    {
        LampAttackModel.OnLampAttackEvent -= TMPHandleLampAttack;
        _movement.OnFallEndedEvent -= OnFallEndedHandle;
    }

    private void OnFallEndedHandle()
    {
        gameObject.SetActive(false);
    }

    public void Initialize(Vector3 startPosition)
    {
        _movement.Initialize(startPosition);
        _presentation.Initialize();
        ReadyToLampDamage = false;
        // Presentation setup
    }

    public override void StartAttack()
    {
        ReadyToCollide = true;
        ReceivedLampAttack = false;
        _movement.TriggerAttack();
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
        Debug.LogWarning("Moth Projectile: Lamp collision penetrated incorrectly.");
    }

    public override void ReceiveDamage(int damage)
    {
        if (damage < 1f) return;

        ReceivedLampAttack = true;
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
}
