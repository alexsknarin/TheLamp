using System;
using System.Collections.Generic;
using UnityEngine;

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
    
    // Dependencies
    private IAnalyticsService _analyticsService;

    public static Transform LampTransform;
    
    private List<EnemyBase> _stickyEnemies;
    private bool _isAssessingDamage = false;
    private bool _isDead = false;
    private Vector3 _enemyPosition;
    
    public static event Action<EnemyBase> OnLampDamagedEvent;
    public static event Action<EnemyBase> OnLampDeadEvent;
    public static event Action<EnemyBase> OnLampCollidedWithStickyEnemyEvent;
    
    public void Construct(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    private void OnEnable()
    {
        /*
        _lampCollisionHandler.OnLampCollidedEnemyEvent += RegisterPotentialDamage;
        _lampCollisionHandler.OnExitLampCollisionEnemyEvent += EnemyExitCollisionHandle;
        _lampStickZoneCollisionHandler.OnCollidedWithStickyEnemyEvent += StickyEnemyEnterCollisionHandle;
        _lampAttackExitZoneCollisionHandler.OnExitAttackExitZoneEvent += AssessDamage;
        _lampStatsManager.OnHealthChangeEvent += HandleUpdateHealth;
        _lampStatsManager.OnHealthUpgradedEvent += HandleUpgradeHealth;
        _lampStatsManager.OnAttackDistanceUpgradedEvent += HandleAttackDistanceUpgrade;
        
        Megabeetle.OnStickAttackedEvent += HandleStickAttack;
        */
    }

    private void OnDisable()
    {
        /*
        _lampCollisionHandler.OnLampCollidedEnemyEvent -= RegisterPotentialDamage;
        _lampCollisionHandler.OnExitLampCollisionEnemyEvent -= EnemyExitCollisionHandle;
        _lampStickZoneCollisionHandler.OnCollidedWithStickyEnemyEvent -= StickyEnemyEnterCollisionHandle;
        _lampAttackExitZoneCollisionHandler.OnExitAttackExitZoneEvent -= AssessDamage;
        _lampStatsManager.OnHealthChangeEvent -= HandleUpdateHealth;
        _lampStatsManager.OnHealthUpgradedEvent -= HandleUpgradeHealth;
        _lampStatsManager.OnAttackDistanceUpgradedEvent -= HandleAttackDistanceUpgrade;
        
        Megabeetle.OnStickAttackedEvent += HandleStickAttack;
        */
    }

    public void Initialize()
    {
        _isDead = false;
        _lampStatsManager.Initialize();
        _lampAttackModel.Initialize();
        _lampMovement.Initialize();
        _lampPresentation.Initialize(_lampStatsManager.DamageWeights, _lampStatsManager.LampImpactPointsData);
        _lampPresentation.SetAttackDistance(_lampStatsManager.CurrentAttackDistance);
        if (_stickyEnemies == null)
        {
            _stickyEnemies = new List<EnemyBase>();
        }
        else
        {
            _stickyEnemies.Clear();
        }
        _attackBlockerCount = 0;
        LampTransform = transform;
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
        MoveLamp(enemy);
        OnLampCollidedWithStickyEnemyEvent?.Invoke(enemy);
    }
    
    private void EnemyExitCollisionHandle(EnemyBase enemy)
    {
        if (enemy.EnemyType == EnemyType.Ladybug || enemy.EnemyType == EnemyType.Megabeetle)
        {
            if (_stickyEnemies.Contains(enemy))
            {
                _stickyEnemies.Remove(enemy);
                _attackBlockerCount = _stickyEnemies.Count;
                if( _attackBlockerCount <= 0)
                {
                    _attackBlockerCount = 0;
                    _lampAttackModel.RemoveAttackBlocker();
                    _lampPresentation.DisableBlockedMode(_isDead);
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
        
        Vector3 impactPoint = enemy.ProvideImpactPoint();
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
            OnLampDeadEvent?.Invoke(enemy);
        }
        else
        {
            _lampPresentation.StartDamageState();
            _analyticsService.SubmitLampDamageEvent(enemy);
            OnLampDamagedEvent?.Invoke(enemy);    
        }
        MoveLamp(enemy);
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
    
    private void MoveLamp(EnemyBase enemy)
    {
        Vector3 enemyPosition;
        if (enemy.GetType() == typeof(Dragonfly))
        {
            enemyPosition = ((Dragonfly)enemy).ProvideImpactPoint(); 
        }
        else
        {
            enemyPosition = enemy.gameObject.transform.position; // TODO - unify for all enemies
        }
        float attackDirection = -(enemyPosition - transform.position).x * 2;
        _lampMovement.AddForce(attackDirection);
    }
    
    public void PlayIntro(float duration)
    {
        _lampPresentation.StartIntroState(duration, _lampStatsManager.CurrentHealth, _lampStatsManager.MaxHealth);
    }
    
    private void HandleStickAttack(EnemyBase enemy)
    {
        ApplyDamage(enemy);
    }
    
    private void HandleAttackDistanceUpgrade()
    {
        _lampPresentation.SetAttackDistance(_lampStatsManager.CurrentAttackDistance);
        _lampPresentation.StartAttackDistanceUpgradeState();
    }
}