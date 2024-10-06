using UnityEngine;

public class DragonflyProjectileMoth : EnemyBase
{
    [SerializeField] private EnemyTypes _enemyType = EnemyTypes.DragonflyProjectile;
    [SerializeField] private DragonflyProjectileMovementMoth _movement;
    public override EnemyTypes EnemyType => _enemyType;
    private bool _isDead = false;

    private void OnEnable()
    {
        LampAttackModel.OnLampAttack += TMPHandleLampAttack;
    }
    
    private void OnDisable()
    {
        LampAttackModel.OnLampAttack -= TMPHandleLampAttack;
    }


    public void Initialize(Vector3 startPosition)
    {
        _movement.Initialize(startPosition);
        _isDead = false;
        ReadyToLampDamage = false;
        // Presentation setup
    }

    public override void AttackStart()
    {
        ReadyToCollide = true;
        ReceivedLampAttack = false;
        _movement.TriggerAttack();
    }

    public override void HandleEnteringAttackZone(Collider2D collider)
    {
        ReadyToLampDamage = true;
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
        Debug.LogWarning("Moth Projectile: Lamp collision penetrated incorrectly.");
    }

    public override void ReceiveDamage(int damage)
    {
        ReceivedLampAttack = true;
        _isDead = true;
        _movement.TriggerFall();
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
