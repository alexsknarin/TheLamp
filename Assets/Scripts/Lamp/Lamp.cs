using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Lamp : MonoBehaviour, IInitializable
{
    [SerializeField] private LampAttackModel _lampAttackModel;
    [SerializeField] private LampPresentation _lampPresentation;
    [SerializeField] private LampCollisionHandler _lampCollisionHandler;
    [SerializeField] private LampMovement _lampMovement;
    [SerializeField] private LampStickZoneCollisionHandler _lampStickZoneCollisionHandler;
    [SerializeField] private LampAttackExitZoneCollisionHandler _lampAttackExitZoneCollisionHandler;    
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _currentHealth;
    [SerializeField] private int _attackBlokerCount;
    private List<EnemyBase> _stickyEnemies;
    private bool _isAssessingDamage = false;
    private Vector3 _enemyPosition;
    
    public static event Action<EnemyBase> OnLampDamaged;
    public static event Action<EnemyBase> OnLampDead;
    public static event Action<EnemyBase> OnLampCollidedWithStickyEnemy;

    private void OnEnable()
    {
        _lampCollisionHandler.OnLampCollidedEnemy += RegisterPotentialDamage;
        _lampCollisionHandler.OnExitLampCollisionEnemy += EnemyExitCollisionHandle;
        _lampStickZoneCollisionHandler.OnCollidedWithStickyEnemy += StickyEnemyEnterCollisionHandle;
        _lampAttackExitZoneCollisionHandler.OnExitAttackExitZone += AssessDamage;
    }
    
    private void OnDisable()
    {
        _lampCollisionHandler.OnLampCollidedEnemy -= RegisterPotentialDamage;
        _lampCollisionHandler.OnExitLampCollisionEnemy -= EnemyExitCollisionHandle;
        _lampStickZoneCollisionHandler.OnCollidedWithStickyEnemy -= StickyEnemyEnterCollisionHandle;
        _lampAttackExitZoneCollisionHandler.OnExitAttackExitZone -= AssessDamage;
    }

    public void Initialize()
    {
        _lampAttackModel.Initialize();
        _lampMovement.Initialize();
        _lampPresentation.Initialize();
        _currentHealth = _maxHealth;
        if (_stickyEnemies == null)
        {
            _stickyEnemies = new List<EnemyBase>();
        }
        else
        {
            _stickyEnemies.Clear();
        }
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
                _lampPresentation.UpdateHealthBar((float)_currentHealth / _maxHealth);
                if (_currentHealth <= 0)
                {
                    OnLampDead?.Invoke(enemy);
                }
                else
                {
                    _lampPresentation.StartDamageState();
                    OnLampDamaged?.Invoke(enemy);    
                }
                MoveLamp();
            }
        }
    }
    
    private void MoveLamp()
    {
        float attackDirection = -(_enemyPosition - transform.position).x * 2;
        _lampMovement.AddForce(attackDirection);
    }
    
    public void PlayIntro(float duration)
    {
        _lampPresentation.StartIntroState(duration, 1f); // TODO: support loading health from the last session
    }

    public void PlayDeath(float duration)
    {
        _lampPresentation.StartDeathState(duration);
    }
}