using System;
using UnityEngine;
using UnityEngine.Pool;

public class Enemy : MonoBehaviour, IInitializable
{
    [SerializeField] private EnemyTypes _enemyType;
    public EnemyTypes EnemyType => _enemyType;
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _currentHealth;
    [SerializeField] private EnemyMovement _enemyMovement;
    [SerializeField] private EnemyPresentation _enemyPresentation;
    
    public bool ReadyToAttack { get; private set; }
    public bool ReadyToCollide { get; private set; }
    public bool ReadyToLampDamage { get; private set; }
    public bool ReceivedLampAttack { get; private set; }
    
    private IObjectPool<Enemy> _objectPool;
    public IObjectPool<Enemy> ObjectPool
    {
        set => _objectPool = value;
    }

    public static event Action<Enemy> OnEnemyDeath;
    public static event Action<Enemy> OnEnemyDeactivated;
    public static event Action<Enemy> OnEnemyDamaged;

    private void OnEnable()
    {
        _enemyMovement.OnPreAttackStart += PreAttackStart;
        _enemyMovement.OnPreAttackEnd += _enemyPresentation.PreAttackEnd;
        _enemyMovement.OnEnemyDeactivated += OnDeactivated;
        _enemyMovement.OnMovementReset += OnMovementReset;
    }
    
    private void OnDisable()
    {
        _enemyMovement.OnPreAttackStart -= PreAttackStart;
        _enemyMovement.OnPreAttackEnd -= _enemyPresentation.PreAttackEnd;
        _enemyMovement.OnEnemyDeactivated -= OnDeactivated;
        _enemyMovement.OnMovementReset -= OnMovementReset;
    }
    
    public void Initialize()
    {
        _enemyMovement.Initialize();
        _enemyPresentation.Initialize();
        _currentHealth = _maxHealth;
        ReadyToAttack = false;
        ReadyToCollide = false;
        ReadyToLampDamage = false;
        ReceivedLampAttack = false;
    }
    
    private void OnMovementReset()
    {
        _enemyPresentation.Initialize();
    }
    
    public void UpdateAttackAvailability()
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
                if ((x < 0 && y < 0.36f) || (x > 0 && y < 1.95f))
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
    
    public void SpreadStart()
    {
        _enemyMovement.TriggerSpread();
    }
   
    public void AttackStart()
    {
        _enemyMovement.TriggerAttack();
    }
    
    private void PreAttackStart()
    {
        ReceivedLampAttack = false;
        _enemyPresentation.PreAttackStart();
        ReadyToAttack = false;
        ReadyToCollide = true;
    }
    
    public void HandleEnteringAttackZone()
    {
        if (_enemyMovement.State == EnemyStates.Attack || _enemyType == EnemyTypes.Ladybug)
        {
            ReadyToLampDamage = true;    
        }
    }
    
    public void HandleCollisionWithLamp()
    {
        _enemyMovement.TriggerFall();
        ReadyToCollide = false;
        ReadyToLampDamage = true;
    }
    
    public void HandleExitingAttackExitZone()
    {
        ReadyToLampDamage = false;
    }
    
    public void HandleCollisionWithStickZone()
    {
        _enemyMovement.TriggerStick();
    }

    public void ReceiveDamage(int damage)
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
            OnEnemyDeath?.Invoke(this);
            _enemyPresentation.DeathFlash();
        }    
    }

    private void OnDeactivated()
    {
        OnEnemyDeactivated?.Invoke(this);
        _objectPool.Release(this);
    }
}
