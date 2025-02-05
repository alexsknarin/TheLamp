using UnityEngine;

public class FFlyPresentation : MonoBehaviour
{
    [SerializeField] private FFly _fly;
    [SerializeField] private FFlyMovement _movement;
    [SerializeField] private PreAttackFlash _preAttackFlash;
    [SerializeField] private DamageFlash _damageFlash;
    [SerializeField] private DeathFlash _deathFlash;
    [SerializeField] private HealthIndication _healthIndication;
    [SerializeField] private TrailResetHandler _trailResetHandler;
    
    public void Initialize()
    {
        _preAttackFlash.Initialize();
        _damageFlash.Initialize();
        _deathFlash.Initialize();
        _healthIndication.Initialize();
        
        _movement.PreAttackStarted += OnPreAttackStarted;
        _movement.PreAttackEnded += OnPreAttackEnded;
        _fly.Started += OnFlyStarted;
        _fly.Damaged += OnFlyDamaged;
        _fly.HealthChanged += _healthIndication.Refresh;
        _fly.Dead += OnFlyDead;
        
    }

    private void OnDestroy()
    {
        _movement.PreAttackStarted -= OnPreAttackStarted;
        _movement.PreAttackEnded -= OnPreAttackEnded;
        _fly.Started -= OnFlyStarted;
        _fly.Damaged -= OnFlyDamaged;
        _fly.HealthChanged -= _healthIndication.Refresh;
        _fly.Dead -= OnFlyDead;
    }

    private void OnFlyStarted()
    {
        _trailResetHandler.Initialize();
        _deathFlash.Initialize();
    }

    private void OnPreAttackStarted()
    {
        _preAttackFlash.PreAttackStart();
    }

    private void OnPreAttackEnded()
    {
        _preAttackFlash.PreAttackEnd();
    }

    private void OnFlyDamaged()
    {
        _damageFlash.Play();
    }

    private void OnFlyDead()
    {
        _deathFlash.Play();
    }
}
