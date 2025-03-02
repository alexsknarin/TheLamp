using UnityEngine;

public class FMegamothlingPresentation : MonoBehaviour
{
    [SerializeField] private FMegamothling _megamothling;
    [SerializeField] private FMegamothlingMovement _movement;
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
        _megamothling.Started += OnMothlingStarted;
        _megamothling.Damaged += OnMothlingDamaged;
        _megamothling.Dead += OnMothlingDead;
        _megamothling.HealthChanged += _healthIndication.Refresh;
        
    }

    private void OnDestroy()
    {
        _movement.PreAttackStarted -= OnPreAttackStarted;
        _movement.PreAttackEnded -= OnPreAttackEnded;
        _movement.SpreadStateEnded -= _trailResetHandler.Initialize;
        _megamothling.Started -= OnMothlingStarted;
        _megamothling.Damaged -= OnMothlingDamaged;
        _megamothling.Dead -= OnMothlingDead;
        _megamothling.HealthChanged -= _healthIndication.Refresh;
    }

    private void OnMothlingStarted()
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

    private void OnMothlingDamaged()
    {
        _damageFlash.Play();
    }

    private void OnMothlingDead()
    {
        _deathFlash.Play();
    }
}
