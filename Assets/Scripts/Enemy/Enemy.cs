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
    private IInitializable _enemyPresentationInitializer;
    [SerializeField] private EnemyCollisionHandler _enemyCollisionHandler;
    public bool ReadyToAttack { get; private set; }
    private IObjectPool<Enemy> _objectPool;
    public IObjectPool<Enemy> ObjectPool
    {
        set => _objectPool = value;
    }

    public static event Action<Enemy> OnEnemyDeath;
    public static event Action<Enemy> OnEnemyDeactivated;
    public static event Action OnEnemyDamaged;

    private void OnEnable()
    {
        _enemyMovement.OnPreAttackStart += PreAttack;
        _enemyMovement.OnPreAttackEnd += _enemyPresentation.PreAttackEnd;
        _enemyMovement.OnStateChange += UpdateAttackAvailability;
        _enemyMovement.OnEnemyDeactivated += OnDeactivated;
        _enemyCollisionHandler.OnCollidedWithLamp += Fall;
    }
    
    private void OnDisable()
    {
        _enemyMovement.OnPreAttackStart -= PreAttack;
        _enemyMovement.OnPreAttackEnd -= _enemyPresentation.PreAttackEnd;
        _enemyMovement.OnStateChange -= UpdateAttackAvailability;
        _enemyMovement.OnEnemyDeactivated -= OnDeactivated;
        _enemyCollisionHandler.OnCollidedWithLamp -= Fall;
    }
    
    public void Initialize()
    {
        _enemyMovement.Initialize();
        _enemyPresentationInitializer = _enemyPresentation;
        _enemyPresentationInitializer.Initialize();
        _currentHealth = _maxHealth;
        ReadyToAttack = false;
    }
    
    private void UpdateAttackAvailability()
    {
        if((_enemyType == EnemyTypes.Fly || _enemyType == EnemyTypes.Firefly) && _enemyMovement.State == EnemyStates.Patrol)
        {
            if(transform.position.y < 0.92f)
            {
                ReadyToAttack = true;
            }
        }
        else if(_enemyType == EnemyTypes.Moth && _enemyMovement.State == EnemyStates.Hover)
        {
            if(transform.position.y < 0.0f )
            {
                ReadyToAttack = true;
            }
            else
            {
                if(Mathf.Abs(transform.position.x) > 0.7f)
                {
                    ReadyToAttack = true;
                }
            }
        }
        else
        {
            ReadyToAttack = false;
        }
    }
   
    public void Attack()
    {
        _enemyMovement.TriggerAttack();
    }
    
    private void PreAttack()
    {
        _enemyCollisionHandler.EnableCollider();        
        _enemyPresentation.PreAttackStart();
    }
    
    private void Fall()
    {
        if (_enemyType != EnemyTypes.Ladybug)
        {
            _enemyCollisionHandler.DisableCollider();
            _enemyMovement.TriggerFall();    
        }
    }

    public void ReceiveDamage(int damage)
    {
        _currentHealth -= damage;
        if (_currentHealth > 0)
        {
            OnEnemyDamaged?.Invoke();
            _enemyPresentation.DamageFlash();
        }
        else
        {
            _currentHealth = 0;
            _enemyMovement.TriggerDeath();
            _enemyPresentation.DeathFlash();
            OnEnemyDeath?.Invoke(this);
        }
    }
    
    private void OnDeactivated()
    {
        OnEnemyDeactivated?.Invoke(this);
        _objectPool.Release(this);
        // gameObject.SetActive(false);
    }
}
