using UnityEngine;

public class Megamothling : BossBase
{
    [SerializeField] private EnemyType _enemyType;
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _currentHealth;
    [SerializeField] private MegamothlingMovement _enemyMovement;
    [SerializeField] private MegamothlingPresentation _enemyPresentation;
    private bool _isDead = false;
    public override EnemyType EnemyType => _enemyType;

    private void OnEnable()
    {
        _enemyMovement.PreAttackStarted += OnPreAttackStarted;
        _enemyMovement.PreAttackEnded += OnPreAttackEnded;
        _enemyMovement.AttackEnded += OnAttackEnded;
        _enemyMovement.MovementReseted += OnMovementReseted;
        _enemyMovement.StickStarted += OnStickStarted;
        _enemyMovement.DeathStateEnded += OnDeathStateEnded;
    }
    
    private void OnDisable()
    {
        _enemyMovement.PreAttackStarted -= OnPreAttackStarted;
        _enemyMovement.PreAttackEnded -= OnPreAttackEnded;
        _enemyMovement.AttackEnded -= OnAttackEnded;
        _enemyMovement.MovementReseted -= OnMovementReseted;
        _enemyMovement.StickStarted -= OnStickStarted;
        _enemyMovement.DeathStateEnded -= OnDeathStateEnded;
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

    public override void Reset()
    {
        ReceivedLampAttack = false;
        _currentHealth = _maxHealth;
        _isDead = false;
        _enemyPresentation.Initialize();
        _enemyMovement.MovementReset();
        gameObject.SetActive(false);
    }

    public override void Play()
    {
        gameObject.SetActive(true);
        _enemyPresentation.ResetTrail();
        _enemyMovement.Play();
    }

    public override void UpdateAttackAvailability()
    {
        float x = transform.position.x;
        float y = transform.position.y;
        
        ReadyToAttack = false;
        
        if (_enemyType == EnemyType.Megamothling && _enemyMovement.State == EnemyState.Patrol)
        {
            if ((y < 0.0f) || (Mathf.Abs(x) > 2.1f && y > 0.0f))
            {
                ReadyToAttack = true;
            }
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

    public override void HandleEnteringAttackZone()
    {
        if (_enemyMovement.State == EnemyState.Attack || _enemyType == EnemyType.Ladybug)
        {
            ReadyToLampDamage = true;
        }
    }

    public override void HandleCollisionWithLamp()
    {
        ReadyToCollide = false;
        _enemyMovement.TriggerFall();
    }

    public override void HandleCollisionWithStickZone()
    {
        _enemyMovement.TriggerStick();
    }

    public override void ReceiveDamage(int damage)
    {
        ReadyToLampDamage = false;
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

    public override void ReturnToPool() { }

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

    private void OnMovementReseted()
    {
        _enemyPresentation.Initialize();
    }

    private void OnStickStarted()
    {
        IsStick = true;
    }

    private void OnDeathStateEnded()
    {
        OnDeathInvoke();
        _enemyMovement.MovementReset();
        _enemyPresentation.Initialize();
        gameObject.SetActive(false);
    }
}
