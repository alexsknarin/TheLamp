using UnityEngine;

public class PlayerEnemyInteractionHandler : MonoBehaviour, IInitializable
{
    [SerializeField] private LampCollisionHandler _lampCollisionHandler;
    [SerializeField] private LampAttackExitZoneCollisionHandler _lampAttackExitZoneCollisionHandler;

    private bool _isAssessingDamage = false;
    private Vector3 _enemyPosition;
    
    public void Initialize()
    {
        _lampCollisionHandler.OnLampCollidedEnemyEvent += RegisterPotentialDamage;
        _lampAttackExitZoneCollisionHandler.OnExitAttackExitZoneEvent += AssessDamage;
    }

    private void OnDestroy()
    {
        _lampCollisionHandler.OnLampCollidedEnemyEvent -= RegisterPotentialDamage;
        _lampAttackExitZoneCollisionHandler.OnExitAttackExitZoneEvent -= AssessDamage;
    }

    public void LampAttack()
    {
        if (_isAssessingDamage)
        {
            Debug.Log("Enemy is damaged!!!");
            _isAssessingDamage = false;
        }
    }
    
    private void RegisterPotentialDamage(EnemyBase enemy)
    {
        _enemyPosition = enemy.transform.position;
        if (!_isAssessingDamage)
        {
            _isAssessingDamage = true;
        }
    }

    private void AssessDamage(EnemyBase enemy)
    {
        if (_isAssessingDamage)
        {
            if (enemy.ReceivedLampAttack)
            {
                _isAssessingDamage = false;
                Debug.Log("Enemy is damaged");
            }
            else
            {
                _isAssessingDamage = false;
                ApplyDamage(enemy);
            }
        }
    }

    private void ApplyDamage(EnemyBase enemy)
    {
        Debug.Log("Lamp is damaged");
    }
}


