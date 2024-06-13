using System;
using UnityEngine;
using UnityEngine.Pool;

public class Megabeetle : BossBase
{
    [SerializeField] private EnemyTypes _enemyType;
    public override EnemyTypes EnemyType => _enemyType;
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _currentHealth;
    [SerializeField] private int _healthToFallThreshold;
    private int _currentHealthToFall;
    [SerializeField] private MegabeetleMovement _enemyMovement;
    [SerializeField] private MegabeetlePresentation _enemyPresentation;
    private bool _isDead = false;
    
    public static Action<EnemyBase> OnStickAttacked;
    
    private void OnEnable()
    {
        _enemyMovement.OnPreAttackStart += OnPreAttackStart;
        _enemyMovement.OnPreAttackEnd += OnPreAttackEnd;
        _enemyMovement.OnAttackEnd += AttackStatusEnable;
        _enemyMovement.OnEnemyDeactivated += OnDeactivated;
        _enemyMovement.OnMovementReset += OnMovementReset;
        _enemyMovement.OnStickStart += StickStatusEnable;
        _enemyMovement.OnDeathStateEnded += HandleDeathMoveStateEnd;
        _enemyMovement.OnStickAttackStateEnded += HandleStickAttack;
    }
    
    private void OnDisable()
    {
        _enemyMovement.OnPreAttackStart -= OnPreAttackStart;
        _enemyMovement.OnPreAttackEnd -= OnPreAttackEnd;
        _enemyMovement.OnAttackEnd -= AttackStatusEnable;
        _enemyMovement.OnEnemyDeactivated -= OnDeactivated;
        _enemyMovement.OnMovementReset -= OnMovementReset;
        _enemyMovement.OnStickStart -= StickStatusEnable;
        _enemyMovement.OnDeathStateEnded -= HandleDeathMoveStateEnd;
        _enemyMovement.OnStickAttackStateEnded += HandleStickAttack;
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
        _currentHealthToFall = 0;
    }
    
    public override void Play()
    {
        _enemyMovement.Play();
        _enemyPresentation.ResetTrail();
    }
    
    public override void Reset()
    {
        ReceivedLampAttack = false;
        _currentHealth = _maxHealth;
        _isDead = false;
        _enemyPresentation.Initialize();
        _enemyMovement.MovementReset();
    }
    
    private void OnMovementReset()
    {
        _enemyPresentation.Initialize();
    }
    
    public override void UpdateAttackAvailability()
    {
        ReadyToAttack = false;
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
        if (_enemyMovement.State == EnemyStates.Attack)
        {
            ReadyToLampDamage = true;    
        }
    }
    
    public override void HandleCollisionWithLamp()
    {
        // ReadyToCollide = false;
        // ReadyToLampDamage = true;
        // _enemyMovement.TriggerFall();
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
        _currentHealthToFall += damage;
        
        if (_currentHealth > 0)
        {
            ReceivedLampAttack = true;
            _enemyPresentation.DamageFlash();
            _enemyPresentation.HealthUpdate(_currentHealth, _maxHealth);
            if (_currentHealthToFall >= _healthToFallThreshold)
            {
                _enemyMovement.TriggerFall();
                _currentHealthToFall = 0;
            }
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
                _currentHealthToFall = 0;
            }
        }    
    }

    public override void ReturnToPool()
    {
    }
    
    private void OnDeactivated()
    {
    }
    
    private void HandleDeathMoveStateEnd()
    {
        OnDeathInvoke();
        _enemyMovement.MovementReset();
        _enemyPresentation.Initialize();
    }
    
    private void HandleStickAttack()
    {
        OnStickAttacked?.Invoke(this);
    }
}
