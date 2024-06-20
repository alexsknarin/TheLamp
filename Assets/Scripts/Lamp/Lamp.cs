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
    [SerializeField] private int _attackBlockerCount;
    [Header("Lamp Stats")]
    [SerializeField] private LampStatsManager _lampStatsManager;
    [Header("Debug/Testing")]
    [SerializeField] private bool _isInvincible;
    
    private List<EnemyBase> _stickyEnemies;
    private bool _isAssessingDamage = false;
    private bool _isDead = false;
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
        _lampStatsManager.OnHealthChange += HandleUpdateHealth;
        _lampStatsManager.OnCooldownChange += HandleUpgradeCooldown;
        _lampStatsManager.OnHealthUpgraded += HandleUpgradeHealth;
        
        Megabeetle.OnStickAttacked += HandleStickAttack;
    }
    
    private void OnDisable()
    {
        _lampCollisionHandler.OnLampCollidedEnemy -= RegisterPotentialDamage;
        _lampCollisionHandler.OnExitLampCollisionEnemy -= EnemyExitCollisionHandle;
        _lampStickZoneCollisionHandler.OnCollidedWithStickyEnemy -= StickyEnemyEnterCollisionHandle;
        _lampAttackExitZoneCollisionHandler.OnExitAttackExitZone -= AssessDamage;
        _lampStatsManager.OnHealthChange -= HandleUpdateHealth;
        _lampStatsManager.OnCooldownChange -= HandleUpgradeCooldown;
        _lampStatsManager.OnHealthUpgraded -= HandleUpgradeHealth;
        
        Megabeetle.OnStickAttacked += HandleStickAttack;
    }

    public void Initialize()
    {
        _isDead = false;
        _lampStatsManager.Initialize();
        _lampAttackModel.Initialize(_lampStatsManager.CurrentColldownTime);
        _lampMovement.Initialize();
        _lampPresentation.Initialize(_lampStatsManager.DamageWeights, _lampStatsManager.LampImpactPointsData);
        if (_stickyEnemies == null)
        {
            _stickyEnemies = new List<EnemyBase>();
        }
        else
        {
            _stickyEnemies.Clear();
        }
        _attackBlockerCount = 0;
    }
    
    public void PlayDeath(float duration)
    {
        _lampPresentation.StartDeathState(duration);
    }
  
    private void StickyEnemyEnterCollisionHandle(EnemyBase enemy)
    {
        _enemyPosition = enemy.transform.position;
        _lampAttackModel.AddAttackBlocker();
        _lampPresentation.EnableBlockedMode();
        if (!_stickyEnemies.Contains(enemy))
        {
            _stickyEnemies.Add(enemy);
            _attackBlockerCount = _stickyEnemies.Count;
        }
        
        enemy.transform.parent = transform;
        enemy.HandleCollisionWithStickZone();
        MoveLamp();
        OnLampCollidedWithStickyEnemy?.Invoke(enemy);
    }
    
    private void EnemyExitCollisionHandle(EnemyBase enemy)
    {
        if (enemy.EnemyType == EnemyTypes.Ladybug || enemy.EnemyType == EnemyTypes.Megabeetle)
        {
            if (_stickyEnemies.Contains(enemy))
            {
                _stickyEnemies.Remove(enemy);
                _attackBlockerCount = _stickyEnemies.Count;
                if( _attackBlockerCount <= 0)
                {
                    _attackBlockerCount = 0;
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

    private void AssessDamage(EnemyBase enemy)
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
                ApplyDamage(enemy);
            }
        }
    }
    
    private void ApplyDamage(EnemyBase enemy)
    {
        if (_isDead)
        {
            return;
        }
        
        Vector3 impactPoint = enemy.transform.position.normalized;
        if (!_isInvincible)
        {
            _lampStatsManager.DecreaseCurrentHealth(1, impactPoint);    
        }
        
        _lampPresentation.UpdateHealthBar(
            _lampStatsManager.NormalizedHealth, 
            _lampStatsManager.CurrentHealth, 
            _lampStatsManager.DamageWeights,
            _lampStatsManager.LampImpactPointsData
        );
        
        if (_lampStatsManager.CurrentHealth <= 0)
        {
            _lampAttackModel.HandleLampDeath();
            _lampPresentation.LastEnemyPosition = enemy.transform.position;
            _isDead = true;
            Debug.Log("Lamp Dead ------------------ <<<<");
            OnLampDead?.Invoke(enemy);
        }
        else
        {
            _lampPresentation.StartDamageState();
            OnLampDamaged?.Invoke(enemy);    
        }
        MoveLamp();
    }
    
    private void HandleUpdateHealth()
    {
        _lampPresentation.UpdateHealthBar(
            _lampStatsManager.NormalizedHealth, 
            _lampStatsManager.CurrentHealth, 
            _lampStatsManager.DamageWeights,
            _lampStatsManager.LampImpactPointsData
        );
    }
    
    private void HandleUpgradeHealth()
    {
        _lampPresentation.UpdateHealthBar(
            _lampStatsManager.NormalizedHealth, 
            _lampStatsManager.CurrentHealth, 
            _lampStatsManager.DamageWeights,
            _lampStatsManager.LampImpactPointsData
        );
        _lampPresentation.UpgradeHealthBar();
    }
        
    
    private void HandleUpgradeCooldown()
    {
        _lampAttackModel.UpgradeCooldownTime(_lampStatsManager.CurrentColldownTime);
    }
    
    private void MoveLamp()
    {
        float attackDirection = -(_enemyPosition - transform.position).x * 2;
        _lampMovement.AddForce(attackDirection);
    }
    
    public void PlayIntro(float duration)
    {
        _lampPresentation.StartIntroState(duration, _lampStatsManager.CurrentHealth, _lampStatsManager.MaxHealth); // TODO: support loading health from the last session
    }
    
    private void HandleStickAttack(EnemyBase enemy)
    {
        ApplyDamage(enemy);
    }
    
}