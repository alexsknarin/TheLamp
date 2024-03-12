using System;
using UnityEngine;

public class Lamp : MonoBehaviour, IInitializable
{
    [SerializeField] private LampAttackModel _lampAttackModel;
    [SerializeField] private LampPresentation _lampPresentation;
    [SerializeField] private LampCollisionHandler _lampCollisionHandler;
    [SerializeField] private LampMovement _lampMovement;
    [SerializeField] private StickZoneCollisionHandler _stickZoneCollisionHandler;
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
        _lampCollisionHandler.OnLampCollidedEnemy += StartAssessForDamage;
        _lampCollisionHandler.OnExitLampCollisionEnemy += EnemyExitCollisionHandle;
       _stickZoneCollisionHandler.OnCollidedWithStickyEnemy += StickyEnemyEnterCollisionHandle;
        Enemy.OnEnemyDamaged += AttackSuccessConfirm;
        Enemy.OnEnemyDeath += AttackSuccessConfirm;
    }
    
    private void OnDisable()
    {
        _lampCollisionHandler.OnLampCollidedEnemy -= StartAssessForDamage;
        _lampCollisionHandler.OnExitLampCollisionEnemy -= EnemyExitCollisionHandle;
        _stickZoneCollisionHandler.OnCollidedWithStickyEnemy -= StickyEnemyEnterCollisionHandle;
        Enemy.OnEnemyDamaged += AttackSuccessConfirm;
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

    // REMAKE with async or coroutine
    private void StartAssessForDamage(Enemy enemy)
    {
        _enemyType = enemy.EnemyType;
        _enemyPosition = enemy.transform.position;
        
        if (enemy.EnemyType == EnemyTypes.Ladybug)
        {
            _lampAttackModel.AddAttackBlocker();
            return;
        }
        if (!_isAssessingDamage)
        {
            _prevTime = Time.time;
            _isAssessingDamage = true;
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
        Debug.Log("Enemy exit collision");
        if (enemy.EnemyType == EnemyTypes.Ladybug)
        {
            _lampAttackModel.RemoveAttackBlocker();
            enemy.transform.parent = null;
        }
    }
    
    public void HandleDamage()
    {
        if (_isAssessingDamage)
        {
            float assessmentPhase = (Time.time - _prevTime) / _damageAssessmentDuration;
            if (assessmentPhase > 1)
            {
                if (_isAttackSuccess)
                {
                    _isAttackSuccess = false;
                    _isAssessingDamage = false;
                }
                else
                {
                    _isAttackSuccess = false;
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
    }
    
    private void MoveLamp()
    {
        float attackDirection = -(_enemyPosition - transform.position).x * 2;
        _lampMovement.AddForce(attackDirection);
    }

    private void Update()
    {
        HandleDamage();
    }
}
