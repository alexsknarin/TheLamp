using System;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

public class Megabeetle : BossBase
{
    [SerializeField] private EnemyType _enemyType;
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _currentHealth;
    [SerializeField] private int _healthToFallThreshold;
    [SerializeField] private MegabeetleMovement _enemyMovement;
    [SerializeField] private MegabeetlePresentation _enemyPresentation;
    public override EnemyType EnemyType => _enemyType;
    public static event Action<EnemyBase> OnStickAttackedEvent;
    private int _currentHealthToFall;
    private bool _isDead = false;
    
    private void OnEnable()
    {
        _enemyMovement.OnPreAttackStartEvent += OnPreAttackStartHandle;
        _enemyMovement.OnPreAttackEndEvent += OnPreAttackEndHandle;
        _enemyMovement.OnAttackEndEvent += AttackStatusEnable;
        _enemyMovement.OnEnemyDeactivatedEvent += OnDeactivatedHandle;
        _enemyMovement.OnMovementResetEvent += OnMovementResetHandle;
        _enemyMovement.OnStickStartEvent += StickStatusEnable;
        _enemyMovement.OnDeathStateEndedEvent += HandleDeathMoveStateEnd;
        _enemyMovement.OnStickAttackStateEndedEvent += HandleStickAttack;
        _enemyMovement.OnTriggerSpreadEvent += MegabeetleTriggerSpread;
    }
    
    private void OnDisable()
    {
        _enemyMovement.OnPreAttackStartEvent -= OnPreAttackStartHandle;
        _enemyMovement.OnPreAttackEndEvent -= OnPreAttackEndHandle;
        _enemyMovement.OnAttackEndEvent -= AttackStatusEnable;
        _enemyMovement.OnEnemyDeactivatedEvent -= OnDeactivatedHandle;
        _enemyMovement.OnMovementResetEvent -= OnMovementResetHandle;
        _enemyMovement.OnStickStartEvent -= StickStatusEnable;
        _enemyMovement.OnDeathStateEndedEvent -= HandleDeathMoveStateEnd;
        _enemyMovement.OnStickAttackStateEndedEvent -= HandleStickAttack;
        _enemyMovement.OnTriggerSpreadEvent -= MegabeetleTriggerSpread;
    }

    private void MegabeetleTriggerSpread()
    {
        OnTriggerSpreadInvoke();
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
    
    private void OnMovementResetHandle()
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
   
    public override void StartAttack()
    {
        _enemyMovement.TriggerAttack();
    }
    
    private void OnPreAttackStartHandle()
    {
        ReceivedLampAttack = false;
        _enemyPresentation.PreAttackStart();
        ReadyToAttack = false;
        IsAttacking = true;
    }
    
    private void OnPreAttackEndHandle()
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
        if (_enemyMovement.State == EnemyState.Attack)
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
    
    public override Vector3 ProvideImpactPoint()
    {
        return transform.position;
    }
    
    private void OnDeactivatedHandle()
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
        OnStickAttackedEvent?.Invoke(this);
    }
}
