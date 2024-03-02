using System;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour, IInitializable
{
    [SerializeField] private EnemyTypes _enemyType;
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _currentHealth;
    [SerializeField] private EnemyMovement _enemyMovement;
    [SerializeField] private EnemyPresentation _enemyPresentation;
    [SerializeField] private EnemyCollisionHandler _enemyCollisionHandler;
    public bool ReadyToAttack { get; private set; }
    public static event Action<Enemy> OnEnemyDeath;
    public static event Action OnEnemyDamaged;

    private void OnEnable()
    {
        _enemyMovement.OnPreAttackStart += PreAttack;
        _enemyMovement.OnPreAttackEnd += _enemyPresentation.PreAttackEnd;
        _enemyMovement.OnStateChange += UpdateAttackAvailability;
        _enemyCollisionHandler.OnCollidedWithLamp += Fall;
        LampAttackModel.OnLampAttack += HandleLampAttack;
    }
    
    private void OnDisable()
    {
        _enemyMovement.OnPreAttackStart -= PreAttack;
        _enemyMovement.OnPreAttackEnd -= _enemyPresentation.PreAttackEnd;
        _enemyMovement.OnStateChange -= UpdateAttackAvailability;
        _enemyCollisionHandler.OnCollidedWithLamp -= Fall;
        LampAttackModel.OnLampAttack -= HandleLampAttack;
    }
    
    private void UpdateAttackAvailability()
    {
        if(_enemyType == EnemyTypes.Fly && _enemyMovement.State == EnemyStates.Patrol)
        {
            if(transform.position.y < 0.92f)
            {
                ReadyToAttack = true;
            }
        }
        if(_enemyType == EnemyTypes.Moth && _enemyMovement.State == EnemyStates.Patrol)
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
        else if (_enemyType == EnemyTypes.Moth && _enemyMovement.State == EnemyStates.Hover)
        {
            ReadyToAttack = true;
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
        _enemyCollisionHandler.DisableCollider();
        _enemyMovement.TriggerFall();
    }
    
    private void HandleLampAttack(int attackPower, float currentPower, float attackDuration, float attackDistance)
    {
        if (gameObject.activeInHierarchy)
        {
            Vector3 current2dPosition = transform.position;
            current2dPosition.z = 0;
            if(current2dPosition.magnitude < attackDistance)
            {
                RecieveDamage(attackPower);
                OnEnemyDamaged?.Invoke();
            }
        }
    }
    
    public void RecieveDamage(int damage)
    {
        _currentHealth -= damage;
        if (_currentHealth > 0)
        {
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
    
    public void Initialize()
    {
        _enemyMovement.Initialize();
        _currentHealth = _maxHealth;
        ReadyToAttack = false;
    }
}
