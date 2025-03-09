using UnityEngine;

public class FMegabeetlePresentation : MonoBehaviour
{
    [SerializeField] private FMegabeetle _megabeetle;
    [SerializeField] private FMegabeetleMovement _movement;
    [SerializeField] private PreAttackFlash _preAttackFlash;
    [SerializeField] private LadybugDamageFlash _damageFlash;
    [SerializeField] private DeathFlash _deathFlash;
    [SerializeField] private HealthIndication _healthIndication;
    [SerializeField] private TrailResetHandler _trailResetHandler;

    public void Initialize()
    {
        _preAttackFlash.Initialize();
        _damageFlash.Initialize();
        _deathFlash.Initialize();
        _healthIndication.Initialize();
        _trailResetHandler.Initialize();
        
        _movement.PreAttackStarted += OnPreAttackStarted;
        _movement.PreAttackEnded += OnPreAttackEnded;
        _movement.FallEnded += OnFallEnded;
        _megabeetle.Started += OnLadybugStarted;
        _megabeetle.Damaged += OnLadybugDamaged;
        _megabeetle.HealthChanged += _healthIndication.Refresh;
        _megabeetle.Dead += OnLadybugDead;
    }

    private void OnDestroy()
    {
        _movement.PreAttackStarted -= OnPreAttackStarted;
        _movement.PreAttackEnded -= OnPreAttackEnded;
        _movement.FallEnded -= OnFallEnded;
        _megabeetle.Started -= OnLadybugStarted;
        _megabeetle.Damaged -= OnLadybugDamaged;
        _megabeetle.HealthChanged -= _healthIndication.Refresh;
        _megabeetle.Dead -= OnLadybugDead;
    }

    private void OnLadybugStarted()
    {
        _trailResetHandler.Initialize();
        _deathFlash.Initialize();
    }

    private void OnLadybugDamaged()
    {
        _damageFlash.Play();
    }

    private void OnPreAttackStarted()
    {
        // TODO: set subscription directly to the methods in presentation and other sub classes 
        _preAttackFlash.PreAttackStart();
    }

    private void OnPreAttackEnded()
    {
        _preAttackFlash.PreAttackEnd();
    }

    private void OnLadybugDead()
    {
        _deathFlash.Play();
    }

    private void OnFallEnded()
    {
        _trailResetHandler.Initialize();
    }
}
