using UnityEngine;

public class FMothPresentation : MonoBehaviour
{
    [SerializeField] private FMoth _moth;
    [SerializeField] private FMothMovement _movement;
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
        _movement.SpreadStateEnded += _trailResetHandler.Initialize;
        _moth.Started += OnFlyStarted;
        _moth.Damaged += OnFlyDamaged;
        _moth.HealthChanged += _healthIndication.Refresh;
        _moth.Dead += OnFlyDead;
        
    }

    private void OnDestroy()
    {
        _movement.PreAttackStarted -= OnPreAttackStarted;
        _movement.PreAttackEnded -= OnPreAttackEnded;
        _movement.SpreadStateEnded -= _trailResetHandler.Initialize;
        _moth.Started -= OnFlyStarted;
        _moth.Damaged -= OnFlyDamaged;
        _moth.HealthChanged -= _healthIndication.Refresh;
        _moth.Dead -= OnFlyDead;
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
