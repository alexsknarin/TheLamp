using System;
using UnityEngine;
using UnityEngine.Pool;

public class Megamothling : BossBase
{
    [SerializeField] private EnemyTypes _enemyType;
    public override EnemyTypes EnemyType => _enemyType;
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _currentHealth;
    [SerializeField] private MegamothlingMovement _enemyMovement;
    [SerializeField] private MegamothlingPresentation _enemyPresentation;
    private bool _isDead = false;

    public static event Action<Enemy> OnEnemyDeactivated;
    public static event Action<Enemy> OnEnemyDamaged;

    private void OnEnable()
    {
        _enemyMovement.OnPreAttackStart += OnPreAttackStart;
        _enemyMovement.OnPreAttackEnd += OnPreAttackEnd;
        _enemyMovement.OnAttackEnd += AttackStatusEnable;
        _enemyMovement.OnMovementReset += OnMovementReset;
        _enemyMovement.OnStickStart += StickStatusEnable;
        _enemyMovement.OnDeathStateEnded += HandleDeathMoveStateEnd;
    }
    
    private void OnDisable()
    {
        _enemyMovement.OnPreAttackStart -= OnPreAttackStart;
        _enemyMovement.OnPreAttackEnd -= OnPreAttackEnd;
        _enemyMovement.OnAttackEnd -= AttackStatusEnable;
        _enemyMovement.OnMovementReset -= OnMovementReset;
        _enemyMovement.OnStickStart -= StickStatusEnable;
        _enemyMovement.OnDeathStateEnded -= HandleDeathMoveStateEnd;
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
        gameObject.SetActive(false);
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
        
        if (_enemyType == EnemyTypes.Megamothling && _enemyMovement.State == EnemyStates.Patrol)
        {
            if ((y < 0.0f) || (Mathf.Abs(x) > 1.1f && y > 0.0f))
            {
                ReadyToAttack = true;
            }
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
    }
    
    public override void Reset()
    {
        ReceivedLampAttack = false;
        _currentHealth = _maxHealth;
        _isDead = false;
        _enemyPresentation.Initialize();
        _enemyMovement.MovementReset();
    }

    public override void Play()
    {
        gameObject.SetActive(true);
        _enemyPresentation.ResetTrail();
        _enemyMovement.Play();
    }
    
    private void HandleDeathMoveStateEnd()
    {
        OnDeathInvoke();
        gameObject.SetActive(false);
    }
}
