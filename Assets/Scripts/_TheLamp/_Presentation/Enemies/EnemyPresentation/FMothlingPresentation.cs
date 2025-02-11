using System;
using UnityEngine;

public class FMothlingPresentation : MonoBehaviour, IInitializable
{
    [SerializeField] private FMothling _mothling;
    [SerializeField] private FMothlingMovement _movement;
    [SerializeField] private PreAttackFlash _preAttackFlash;
    [SerializeField] private DamageFlash _damageFlash;
    [SerializeField] private DeathFlash _deathFlash;
    [SerializeField] private TrailResetHandler _trailResetHandler;
    
    public void Initialize()
    {
        _preAttackFlash.Initialize();
        _damageFlash.Initialize();
        _deathFlash.Initialize();
        
        _movement.PreAttackStarted += OnPreAttackStarted;
        _movement.PreAttackEnded += OnPreAttackEnded;
        _movement.SpreadStateEnded += _trailResetHandler.Initialize;
        _mothling.Started += OnMothlingStarted;
        _mothling.Damaged += OnMothlingDamaged;
        _mothling.Dead += OnMothlingDead;
        
    }

    private void OnDestroy()
    {
        _movement.PreAttackStarted -= OnPreAttackStarted;
        _movement.PreAttackEnded -= OnPreAttackEnded;
        _movement.SpreadStateEnded -= _trailResetHandler.Initialize;
        _mothling.Started -= OnMothlingStarted;
        _mothling.Damaged -= OnMothlingDamaged;
        _mothling.Dead -= OnMothlingDead;
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
