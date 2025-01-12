using System;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

public class Enemy : EnemyBase
{
    [SerializeField] private bool _isStick;
    [SerializeField] private EnemyType _enemyType;
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _currentHealth;
    [SerializeField] private EnemyMovement _enemyMovement;
    [SerializeField] private EnemyPresentation _enemyPresentation;
    private bool _isDead = false;
    // private ILampPositionProviderService _lampPositionProviderService;
    private IObjectPool<Enemy> _objectPool;

    // public void Construct(ILampPositionProviderService lampPositionProviderService)
    // {
    //     _lampPositionProviderService = lampPositionProviderService;
    // }

    public static event Action<Enemy> EnemyDeactivated;
    public static event Action<Enemy> EnemyDamaged;

    public IObjectPool<Enemy> ObjectPool
    {
        set => _objectPool = value;
    }
    public override EnemyType EnemyType => _enemyType;


    private void OnEnable()
    {
        _enemyMovement.PreAttackStarted += OnPreAttackStarted;
        _enemyMovement.PreAttackEnded += OnPreAttackEnded;
        _enemyMovement.AttackEnded += OnAttackEnded;
        _enemyMovement.EnemyDeactivated += OnEnemyDeactivated;
        _enemyMovement.MovementReseted += OnMovementReseted;
        _enemyMovement.StickStarted += OnStickStarted;
    }
    
    private void OnDisable()
    {
        _enemyMovement.PreAttackStarted -= OnPreAttackStarted;
        _enemyMovement.PreAttackEnded -= OnPreAttackEnded;
        _enemyMovement.AttackEnded -= OnAttackEnded;
        _enemyMovement.EnemyDeactivated -= OnEnemyDeactivated;
        _enemyMovement.MovementReseted -= OnMovementReseted;
        _enemyMovement.StickStarted -= OnStickStarted;
    }
    
    public override void Initialize()
    {
        Debug.Log($"Enemy {gameObject.name} initialized");
        _enemyMovement.Initialize();
        _enemyPresentation.Initialize();
        _currentHealth = _maxHealth;
        ReadyToAttack = false;
        ReadyToCollide = false;
        ReadyToLampDamage = false;
        ReceivedLampAttack = false;
        IsAttacking = false;
        IsStick = false;
        _isStick = false;
        _isDead = false;
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

    public override void HandleCollisionWithLamp()
    {
        ReadyToCollide = false;
        ReadyToLampDamage = true;
        _enemyMovement.TriggerFall(); // TODO: interface for enemy movement and get implementation back into the  base class
    }

    public override void HandleCollisionWithStickZone()
    {
        _enemyMovement.TriggerStick();
    }

    public override void ReceiveDamage(int damage)
    {
        if (_enemyType != EnemyType.Ladybug)
        {
            ReadyToLampDamage = false; // TODO: better mechanism - separate IStickyDamageable class or somthing
        }
        
        _currentHealth -= damage;

        if (_currentHealth > 0)
        {
            ReceivedLampAttack = true;
            _enemyPresentation.DamageFlash();
            _enemyPresentation.HealthUpdate(_currentHealth, _maxHealth);
            EnemyDamaged?.Invoke(this);
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

    public override void HandleLampDestroyed()
    {
        _enemyMovement.HandleLampDestroyed();
    }

    public override Vector3 ProvideImpactPoint()
    {
        return transform.position;
    }

    
    // Event Handle Methods
    private void OnPreAttackStarted()
    {
        ReceivedLampAttack = false;
        _enemyPresentation.PreAttackStart();
        ReadyToAttack = false;
        IsAttacking = true;
    }

    private void OnPreAttackEnded()
    {
        _enemyPresentation.PreAttackEnd();
        ReadyToCollide = true;
    }

    private void OnAttackEnded()
    {
        IsAttacking = false;
    }

    private void OnEnemyDeactivated()
    {
        EnemyDeactivated?.Invoke(this);
        _objectPool.Release(this);
    }

    private void OnMovementReseted()
    {
        _enemyPresentation.Initialize();
    }

    private void OnStickStarted()
    {
        Debug.Log($"Stick status enabled for {gameObject.name}");
        IsStick = true;
        _isStick = true;
    }
}
