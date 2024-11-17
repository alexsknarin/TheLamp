using System;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

public class Enemy : EnemyBase
{
    [SerializeField] private EnemyType _enemyType;
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _currentHealth;
    [SerializeField] private EnemyMovement _enemyMovement;
    [SerializeField] private EnemyPresentation _enemyPresentation;
    public override EnemyType EnemyType => _enemyType;
    private bool _isDead = false;
    
    private IObjectPool<Enemy> _objectPool;
    public IObjectPool<Enemy> ObjectPool
    {
        set => _objectPool = value;
    }

    public static event Action<Enemy> OnEnemyDeactivatedEvent;
    public static event Action<Enemy> OnEnemyDamagedEvent;

    private void OnEnable()
    {
        _enemyMovement.OnPreAttackStartEvent += OnPreAttackStart;
        _enemyMovement.OnPreAttackEndEvent += OnPreAttackEnd;
        _enemyMovement.OnAttackEndEvent += AttackStatusEnable;
        _enemyMovement.OnEnemyDeactivatedEvent += OnDeactivated;
        _enemyMovement.OnMovementResetEvent += OnMovementReset;
        _enemyMovement.OnStickStartEvent += StickStatusEnable;
    }
    
    private void OnDisable()
    {
        _enemyMovement.OnPreAttackStartEvent -= OnPreAttackStart;
        _enemyMovement.OnPreAttackEndEvent -= OnPreAttackEnd;
        _enemyMovement.OnAttackEndEvent -= AttackStatusEnable;
        _enemyMovement.OnEnemyDeactivatedEvent -= OnDeactivated;
        _enemyMovement.OnMovementResetEvent -= OnMovementReset;
        _enemyMovement.OnStickStartEvent -= StickStatusEnable;
    }
    
    public override void Initialize()
    {
        _enemyMovement.Initialize();
        _enemyPresentation.Initialize();
        _currentHealth = _maxHealth;
        ReadyToAttack = false;
        ReadyToCollide = false;
        ReadyToLampDamage = false;
        ReceivedLampAttack = false;
        IsAttacking = false;
        IsStick = false;
        _isDead = false;
    }
    
    private void OnMovementReset()
    {
        _enemyPresentation.Initialize();
    }
    
    public override void UpdateAttackAvailability()
    {
        float x = transform.position.x;
        float y = transform.position.y;
        
        ReadyToAttack = false;
        
        if(_enemyMovement.State == EnemyState.Spread)
        {
            ReadyToAttack = false;
            return;
        }
        
        if ((_enemyType == EnemyType.Fly || _enemyType == EnemyType.Firefly) &&
            _enemyMovement.State == EnemyState.Patrol )
        {
            if (_enemyMovement.SideDirection < 0)
            {
                if ((x < 0 && y < 0.30f) || (x > 0 && y < 1.65f))
                {
                    ReadyToAttack = true; 
                    return;
                }
            }
            if (_enemyMovement.SideDirection > 0)
            {
                if ((x > 0 && y < 0.30f) || (x < 0 && y < 1.65f))
                {
                    ReadyToAttack = true; 
                    return;
                }
            }
        }

        if (_enemyType == EnemyType.Mothling && _enemyMovement.State == EnemyState.Patrol)
        {
            if ((y < 0.0f) || (Mathf.Abs(x) > 1.1f && y > 0.0f))
            {
                ReadyToAttack = true;
                return;
            }
        }

        if (_enemyType == EnemyType.Moth && _enemyMovement.State == EnemyState.Hover)
        {
            if ((Mathf.Abs(transform.position.x) > 0.7f && transform.position.y <0.85f) || transform.position.y < 0.0f)
            {
                ReadyToAttack = true;
                return;
            }
        }

        if (_enemyType == EnemyType.Spider && _enemyMovement.State == EnemyState.Patrol)
        {
            ReadyToAttack = true;
        }
    }
    
    public override void SpreadStart()
    {
        _enemyMovement.TriggerSpread();
    }
   
    public override void StartAttack()
    {
        _enemyMovement.TriggerAttack();
    }
    
    private void OnPreAttackStart()
    {
        ReceivedLampAttack = false;
        _enemyPresentation.PreAttackStart();
        ReadyToAttack = false;
        IsAttacking = true;
    }
    
    private void OnPreAttackEnd()
    {
        _enemyPresentation.PreAttackEnd();
        ReadyToCollide = true;
    }
    
    private void AttackStatusEnable()
    {
        IsAttacking = false;
    }
    
    private void StickStatusEnable()
    {
        IsStick = true;
    }
    
    public override void HandleEnteringAttackZone()
    {
        ReadyToLampDamage = true;    
    }
    
    public override void HandleCollisionWithLamp()
    {
        ReadyToCollide = false;
        ReadyToLampDamage = true;
        _enemyMovement.TriggerFall();
    }
    
    public override void HandleExitingAttackExitZone()
    {
        ReadyToLampDamage = false;
    }
    
    public override void HandleCollisionWithStickZone()
    {
        _enemyMovement.TriggerStick();
    }

    public override void ReceiveDamage(int damage)
    {
        _currentHealth -= damage;

        if (_currentHealth > 0)
        {
            ReceivedLampAttack = true;
            _enemyPresentation.DamageFlash();
            _enemyPresentation.HealthUpdate(_currentHealth, _maxHealth);
            OnEnemyDamagedEvent?.Invoke(this);
            _enemyMovement.TriggerFall();
        }
        else
        {
            if (!_isDead)
            {
                ReceivedLampAttack = true;
                _currentHealth = 0; 
                _enemyMovement.TriggerDeath();
                OnEnemyDeathInvoke(this);
                _enemyPresentation.DeathFlash();
                _isDead = true;
            }
        }    
    }

    public override void ReturnToPool()
    {
        if (gameObject.activeInHierarchy)
        {
            _objectPool.Release(this);    
        }
    }

    public override Vector3 ProvideImpactPoint()
    {
        return transform.position;
    }

    private void OnDeactivated()
    {
        OnEnemyDeactivatedEvent?.Invoke(this);
        _objectPool.Release(this);
    }
}
