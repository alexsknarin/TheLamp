using System;
using UnityEngine;

public class FLadybug : FEnemy, IStickableWithLamp
{
    [SerializeField] private int _maxHealth = 7;
    [SerializeField] private int _currentHealth;
    [SerializeField] private float _collisionRadius = 0.125f;
    [Header("-- Movement --")]
    [SerializeField] private FLadybugMovement _movement;
    
    public event Action Started;
    public event Action Damaged;
    public event Action<int, int> HealthChanged; 
    public event Action Dead;
    
    public override bool IsReadyToAttack => CheckIsReadyToAttack();

    public bool IsSticked { get; private set; }
    public Vector2 Position => _movement.Position2D;
    public float Radius => _collisionRadius;
    public StickableState StickState { get; private set; }

    public override void Initialize()
    {
        _movement.Initialize();
        // _movement.ReadyToAttackStateStarted += OnReadyToAttackStateStarted;
        // _movement.ReadyToAttackStateEnded += OnReadyToAttackStateEnded;
        _movement.DeathStateEnded += OnDeathStateEnded;
    }

    private void OnDestroy()
    {
        // _movement.ReadyToAttackStateStarted -= OnReadyToAttackStateStarted;
        // _movement.ReadyToAttackStateEnded -= OnReadyToAttackStateEnded;
        _movement.DeathStateEnded -= OnDeathStateEnded;
    }

    public override void Play()
    {
        Debug.Log("******** Ladybug Play Called.");
        
        IsDead = false;
        _currentHealth = _maxHealth;
        _isInAttackReadyMovementState = false;
        IsReadyForDamage = false;
        IsReceivedAttack = false;
        StickState = StickableState.Outside;
        HealthChanged?.Invoke(_currentHealth, _maxHealth);
        _movement.Play();
        Started?.Invoke();
    }

    public override void ReceiveDamage(int damageAmount)
    {
        IsReceivedAttack = true;
        _currentHealth -= damageAmount;
        
        if (_currentHealth <= 0)
        {
            Dead?.Invoke();
            DoDeath();
            IsDead = true;
            IsReadyForDamage = false;
        }
        else
        {
            Debug.Log($"Damage Received: {damageAmount}.");
            Damaged?.Invoke();
            HealthChanged?.Invoke(_currentHealth, _maxHealth);
        }
    }

    private bool CheckIsReadyToAttack()
    {
        if (_isInAttackReadyMovementState)
        {
            return true;
        }
        return false;
    }

    public override void Attack()
    {
        _isInAttackReadyMovementState = false;
        IsReceivedAttack = false;
        _movement.TriggerAttack();
    }

    public override void Spread()
    {
        _movement.TriggerSpread();
    }

    public override void DoDeath()
    {
        Debug.Log("Ladybug is dead.");
        _movement.TriggerDeath();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, _collisionRadius);
    }

    
    // Handle sticky stuff
    public void HandleEnterAttackZone()
    {
        Debug.Log("+++Ladybug is in attack zone.");
        IsSticked = false;
        StickState = StickableState.InAttackZone;
        IsReadyForDamage = true;
    }

    public void HandleStick(Transform lampTransform)
    {
        Debug.Log("+++Ladybug is sticked.");
        IsSticked = true;
        StickState = StickableState.Sticked;
        IsReadyForDamage = true;
        // Play event
        
        _movement.TriggerStick(lampTransform);
        
    }

    public void HandleExitAttackZone()
    {
        Debug.Log("+++Ladybug is out of attack zone.");
        IsSticked = false;
        StickState = StickableState.Outside;
        IsReadyForDamage = false;
        // Most likely dead at this moment
    }
}
