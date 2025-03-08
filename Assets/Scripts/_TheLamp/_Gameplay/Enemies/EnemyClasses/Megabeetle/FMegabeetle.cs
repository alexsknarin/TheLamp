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
        Debug.Log("Megabeetle Play");
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

    public Vector2 Position { get; }
    public float Radius { get; }
    public bool IsSticked { get; }
    public AttackBlockerState AttackBlockState { get; }
    public StickableState StickState { get; }
    public void HandleEnterAttackZone()
    {
        throw new System.NotImplementedException();
    }

    public void HandleStick(Transform lampTransform)
    {
        throw new System.NotImplementedException();
    }

    public void HandleExitAttackZone()
    {
        throw new System.NotImplementedException();
    }

    public void HandleEnterAttackBlockerZone()
    {
        throw new System.NotImplementedException();
    }

    public void HandleLampDestroyed()
    {
        throw new System.NotImplementedException();
    }

    public Vector3 ProvideImpactPoint()
    {
        throw new System.NotImplementedException();
    }
}
