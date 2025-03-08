using UnityEngine;

public class FMegabeetle : FEnemy, IStickableWithLamp
{
    [SerializeField] private float _collisionRadius = 0.3f;
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _currentHealth;
    [SerializeField] private int _healthToFallThreshold;
    [SerializeField] private FMegabeetleMovement _movement;
    private int _currentHealthToFall;
    
    public override void Initialize()
    {
        _movement.Initialize();
    }

    public override void Play()
    {
        IsDead = false;
        _currentHealth = _maxHealth;
        _isInAttackReadyMovementState = false;
        IsReadyForDamage = false;
        IsReceivedLampAttackDamage = false;
        StickState = StickableState.Outside;
        AttackBlockState = AttackBlockerState.Outisde;
        // HealthChanged?.Invoke(_currentHealth, _maxHealth);
        _movement.Play();
        // Started?.Invoke();
    }

    public override void ReceiveDamage(int damageAmount)
    {
        throw new System.NotImplementedException();
    }

    public override void Attack()
    {
        throw new System.NotImplementedException();
    }

    public override void DoDeath()
    {
        throw new System.NotImplementedException();
    }

    public Vector2 Position => _movement.Position2D;
    public float Radius { get; private set; }
    public bool IsSticked { get; private set; }
    public AttackBlockerState AttackBlockState { get; private set; }
    public StickableState StickState { get; private set; }
    
    public void HandleEnterAttackZone()
    {
        IsSticked = false;
        StickState = StickableState.InAttackZone;
        IsReadyForDamage = true;
    }

    public void HandleStick(Transform lampTransform)
    {
        IsSticked = true;
        StickState = StickableState.Sticked;
        AttackBlockState = AttackBlockerState.Sticked;
        IsReadyForDamage = true;
        // Play event
        
        _movement.TriggerStick(lampTransform);
    }

    public void HandleExitAttackZone()
    {
        IsSticked = false;
        StickState = StickableState.Outside;
        IsReadyForDamage = false;
    }

    public void HandleEnterAttackBlockerZone()
    {
        AttackBlockState = AttackBlockerState.Inside;
    }

    public void HandleLampDestroyed()
    {
        throw new System.NotImplementedException();
    }

    public Vector3 ProvideImpactPoint()
    {
        return transform.localPosition;
    }
}
