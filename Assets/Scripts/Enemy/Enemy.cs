using System;
using UnityEngine;
using UnityEngine.Pool;

public class Enemy : EnemyBase
{
    [SerializeField] private EnemyTypes _enemyType;
    public override EnemyTypes EnemyType => _enemyType;
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _currentHealth;
    [SerializeField] private EnemyMovement _enemyMovement;
    [SerializeField] private EnemyPresentation _enemyPresentation;
    
    private IObjectPool<Enemy> _objectPool;
    public IObjectPool<Enemy> ObjectPool
    {
        set => _objectPool = value;
    }

    public static event Action<Enemy> OnEnemyDeactivated;
    public static event Action<Enemy> OnEnemyDamaged;

    private void OnEnable()
    {
        _enemyMovement.OnPreAttackStart += OnPreAttackStart;
        _enemyMovement.OnPreAttackEnd += OnPreAttackEnd;
        _enemyMovement.OnAttackEnd += AttackStatusEnable;
        _enemyMovement.OnEnemyDeactivated += OnDeactivated;
        _enemyMovement.OnMovementReset += OnMovementReset;
        _enemyMovement.OnStickStart += StickStatusEnable;
    }
    
    private void OnDisable()
    {
        _enemyMovement.OnPreAttackStart -= OnPreAttackStart;
        _enemyMovement.OnPreAttackEnd -= OnPreAttackEnd;
        _enemyMovement.OnAttackEnd -= AttackStatusEnable;
        _enemyMovement.OnEnemyDeactivated -= OnDeactivated;
        _enemyMovement.OnMovementReset -= OnMovementReset;
        _enemyMovement.OnStickStart -= StickStatusEnable;
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
        
        if(_enemyMovement.State == EnemyStates.Spread)
        {
            ReadyToAttack = false;
            return;
        }
        
        if ((_enemyType == EnemyTypes.Fly || _enemyType == EnemyTypes.Firefly) &&
            _enemyMovement.State == EnemyStates.Patrol )
        {
            if (_enemyMovement.SideDirection < 0)
            {
                if ((x < 0 && y < 0.32f) || (x > 0 && y < 1.85f))
                {
                    ReadyToAttack = true; 
                    return;
                }
            }
            if (_enemyMovement.SideDirection > 0)
            {
                if ((x > 0 && y < 0.36f) || (x < 0 && y < 2f))
                {
                    ReadyToAttack = true; 
                    return;
                }
            }
        }

        if (_enemyType == EnemyTypes.Moth && _enemyMovement.State == EnemyStates.Hover)
        {
            if ((Mathf.Abs(transform.position.x) > 0.7f && transform.position.y <0.85f) || transform.position.y < 0.0f)
            {
                ReadyToAttack = true;
                return;
            }
        }

        if (_enemyType == EnemyTypes.Spider && _enemyMovement.State == EnemyStates.Patrol)
        {
            ReadyToAttack = true;
        }
    }
    
    public override void SpreadStart()
    {
        _enemyMovement.TriggerSpread();
    }
   
    public override void AttackStart()
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
        if (_enemyMovement.State == EnemyStates.Attack || _enemyType == EnemyTypes.Ladybug)
        {
            ReadyToLampDamage = true;    
        }
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
            OnEnemyDamaged?.Invoke(this);
            _enemyMovement.TriggerFall();
        }
        else
        {
            ReceivedLampAttack = true;
            _currentHealth = 0; 
            _enemyMovement.TriggerDeath();
            OnEnemyDeathInvoke(this);
            _enemyPresentation.DeathFlash();
        }    
    }

    public override void ReturnToPool()
    {
        _objectPool.Release(this);
    }
    
    private void OnDeactivated()
    {
        OnEnemyDeactivated?.Invoke(this);
        _objectPool.Release(this);
    }
}
