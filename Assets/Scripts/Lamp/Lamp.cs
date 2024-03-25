using System;
using System.Collections.Generic;
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
    [SerializeField] private int _attackBlokerCount;
    private List<EnemyBase> _stickyEnemies;
    private bool _isAssessingDamage = false;
    private Vector3 _enemyPosition;
    
    public static event Action OnLampDamaged;
    public static event Action<EnemyBase> OnLampCollidedWithStickyEnemy;

    private void OnEnable()
    {
        _lampCollisionHandler.OnLampCollidedEnemy += RegisterPotentialDamage;
        _lampCollisionHandler.OnExitLampCollisionEnemy += EnemyExitCollisionHandle;
        _stickZoneCollisionHandler.OnCollidedWithStickyEnemy += StickyEnemyEnterCollisionHandle;
        _attackExitZoneCollisionHandler.OnExitAttackExitZone += AssessDamage;
    }
    
    private void OnDisable()
    {
        _lampCollisionHandler.OnLampCollidedEnemy -= RegisterPotentialDamage;
        _lampCollisionHandler.OnExitLampCollisionEnemy -= EnemyExitCollisionHandle;
        _stickZoneCollisionHandler.OnCollidedWithStickyEnemy -= StickyEnemyEnterCollisionHandle;
        _attackExitZoneCollisionHandler.OnExitAttackExitZone -= AssessDamage;
    }

    public void Initialize()
    {
        _lampAttackModel.Initialize();
        _lampMovement.Initialize();
        _lampPresentation.Initialize();
        _currentHealth = _maxHealth;
        _stickyEnemies = new List<EnemyBase>();
        _attackBlokerCount = 0;
    }
  
    private void StickyEnemyEnterCollisionHandle(EnemyBase enemy)
    {
        _enemyPosition = enemy.transform.position;
        _lampAttackModel.AddAttackBlocker();
        _lampPresentation.EnableBlockedMode();
        if (!_stickyEnemies.Contains(enemy))
        {
            _stickyEnemies.Add(enemy);
            _attackBlokerCount = _stickyEnemies.Count;
        }
        
        enemy.transform.parent = transform;
        MoveLamp();
        OnLampCollidedWithStickyEnemy?.Invoke(enemy);
    }
    
    private void EnemyExitCollisionHandle(EnemyBase enemy)
    {
        if (enemy.EnemyType == EnemyTypes.Ladybug)
        {
            if (_stickyEnemies.Contains(enemy))
            {
                _stickyEnemies.Remove(enemy);
                _attackBlokerCount = _stickyEnemies.Count;
                if( _attackBlokerCount <= 0)
                {
                    _attackBlokerCount = 0;
                    _lampAttackModel.RemoveAttackBlocker();
                    _lampPresentation.DisableBlockedMode();
                }    
            }
            enemy.transform.parent = null;
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
    
    public void AssessDamage(EnemyBase enemy)
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