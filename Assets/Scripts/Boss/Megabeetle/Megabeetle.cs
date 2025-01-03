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
    private int _currentHealthToFall;
    private bool _isDead = false;
    public static event Action<EnemyBase> OnStickAttackedEvent;

    public override EnemyType EnemyType => _enemyType;

    private void OnEnable()
    {
        _enemyMovement.PreAttackStarted += OnPreAttackStarted;
        _enemyMovement.PreAttackEnded += OnPreAttackEnded;
        _enemyMovement.AttackEnded += OnAttackEnded;
        _enemyMovement.EnemyDeactivated += OnEnemyDeactivated;
        _enemyMovement.MovementReseted += OnMovementReseted;
        _enemyMovement.StickStarted += OnStickStarted;
        _enemyMovement.DeathStateEnded += OnDeathStateEnded;
        _enemyMovement.StickAttackStateEnded += OnStickAttackStateEnded;
        _enemyMovement.SpreadTriggered += OnSpreadTriggered;
    }

    private void OnDisable()
    {
        _enemyMovement.PreAttackStarted -= OnPreAttackStarted;
        _enemyMovement.PreAttackEnded -= OnPreAttackEnded;
        _enemyMovement.AttackEnded -= OnAttackEnded;
        _enemyMovement.EnemyDeactivated -= OnEnemyDeactivated;
        _enemyMovement.MovementReseted -= OnMovementReseted;
        _enemyMovement.StickStarted -= OnStickStarted;
        _enemyMovement.DeathStateEnded -= OnDeathStateEnded;
        _enemyMovement.StickAttackStateEnded -= OnStickAttackStateEnded;
        _enemyMovement.SpreadTriggered -= OnSpreadTriggered;
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
        gameObject.SetActive(false);
    }

    public override void Play()
    {
        gameObject.SetActive(true);
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
        gameObject.SetActive(false);
    }

    public override void UpdateAttackAvailability()
    {
        ReadyToAttack = false;
    }

    public override void SpreadStart()
    {
        _enemyMovement.TriggerSpread();
    }

    public override void ReturnToPool() { }

    public override void StartAttack()
    {
        _enemyMovement.TriggerAttack();
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

    public override Vector3 ProvideImpactPoint()
    {
        return transform.position;
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

    private void OnMovementReseted()
    {
        _enemyPresentation.Initialize();
    }

    private void OnSpreadTriggered()
    {
        OnTriggerSpreadInvoke();
    }

    private void OnPreAttackStarted()
    {
        ReceivedLampAttack = false;
        _enemyPresentation.PreAttackStart();
        ReadyToAttack = false;
        IsAttacking = true;
    }

    private void OnStickStarted()
    {
        IsStick = true;
    }

    private void OnEnemyDeactivated()
    {
    }

    private void OnAttackEnded()
    {
        IsAttacking = false;
    }

    private void OnDeathStateEnded()
    {
        OnDeathInvoke();
        _enemyMovement.MovementReset();
        _enemyPresentation.Initialize();
        gameObject.SetActive(false);
    }

    private void OnPreAttackEnded()
    {
        _enemyPresentation.PreAttackEnd();
        ReadyToCollide = true;
    }

    private void OnStickAttackStateEnded()
    {
        OnStickAttackedEvent?.Invoke(this);
    }
}
