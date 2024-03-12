using System;
using UnityEngine;

public class Lamp : MonoBehaviour, IInitializable
{
    [SerializeField] private LampAttackModel _lampAttackModel;
    [SerializeField] private LampPresentation _lampPresentation;
    [SerializeField] private LampCollisionHandler _lampCollisionHandler;
    [SerializeField] private LampMovement _lampMovement;
    [SerializeField] private StickZoneCollisionHandler _stickZoneCollisionHandler;
    [SerializeField] private AttackExitZoneCollisionHandler _attackExitZoneCollisionHandler;    
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _currentHealth;
    private bool _isAttackSuccess = false;
    [SerializeField] private float _damageAssessmentDuration = 0.12f;
    private float _prevTime;
    private bool _isAssessingDamage = false;
    private EnemyTypes _enemyType;
    private Vector3 _enemyPosition;
    
    public static event Action OnLampDamaged;
    
    private void OnEnable()
    {
        _lampCollisionHandler.OnLampCollidedEnemy += RegisterPotentialDamage;
        _lampCollisionHandler.OnExitLampCollisionEnemy += EnemyExitCollisionHandle;
        _stickZoneCollisionHandler.OnCollidedWithStickyEnemy += StickyEnemyEnterCollisionHandle;
        _attackExitZoneCollisionHandler.OnExitAttackExitZone += AssessDamage;
        Enemy.OnEnemyDamaged += AttackSuccessConfirm;
        Enemy.OnEnemyDeath += AttackSuccessConfirm;
    }
    
    private void OnDisable()
    {
        _lampCollisionHandler.OnLampCollidedEnemy -= RegisterPotentialDamage;
        _lampCollisionHandler.OnExitLampCollisionEnemy -= EnemyExitCollisionHandle;
        _stickZoneCollisionHandler.OnCollidedWithStickyEnemy -= StickyEnemyEnterCollisionHandle;
        _attackExitZoneCollisionHandler.OnExitAttackExitZone -= AssessDamage;
        Enemy.OnEnemyDamaged -= AttackSuccessConfirm;
        Enemy.OnEnemyDeath -= AttackSuccessConfirm;
    }

    public void Initialize()
    {
        _lampAttackModel.Initialize();
        _lampMovement.Initialize();
        _currentHealth = _maxHealth;
    }

    private void AttackSuccessConfirm(Enemy enemy)
    {
        if(enemy.EnemyType != EnemyTypes.Ladybug)
        {
            _isAttackSuccess = true;
        }
    }
    
    private void StickyEnemyEnterCollisionHandle(Enemy enemy)
    {
        _enemyPosition = enemy.transform.position;
        _lampAttackModel.AddAttackBlocker();
        enemy.transform.parent = transform;
        MoveLamp();
    }
    
    private void EnemyExitCollisionHandle(Enemy enemy)
    {
        if (enemy.EnemyType == EnemyTypes.Ladybug)
        {
            _lampAttackModel.RemoveAttackBlocker();
            enemy.transform.parent = null;
        }
    }
    
    private void RegisterPotentialDamage(Enemy enemy)
    {
        _enemyType = enemy.EnemyType;
        _enemyPosition = enemy.transform.position;
        if (!_isAssessingDamage)
        {
            _isAssessingDamage = true;
        }
    }
    
    public void AssessDamage(Enemy enemy)
    {
        if (_isAssessingDamage)
        {
            if (enemy.ReceivedLampAttack)
            {
                _isAssessingDamage = false;
            }
            else
            {
                _isAssessingDamage = false;
                _currentHealth--;
                _lampPresentation.StartDamageState();
                OnLampDamaged?.Invoke();
                MoveLamp();
                if (_currentHealth <= 0)
                {
                }
            }
        }
    }
    
    private void MoveLamp()
    {
        float attackDirection = -(_enemyPosition - transform.position).x * 2;
        _lampMovement.AddForce(attackDirection);
    }
}