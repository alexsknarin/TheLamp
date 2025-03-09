using UnityEngine;
using UnityEngine.Serialization;

public class SpiderPresentation: MonoBehaviour
{
    [SerializeField] private Spider _spider;
    [SerializeField] private SpiderMovement _movement;
    [SerializeField] private PreAttackFlash _preAttackFlash;
    [SerializeField] private DamageFlash _damageFlash;
    [SerializeField] private DeathFlash _deathFlash;
    [SerializeField] private HealthIndication _healthIndication;
    [SerializeField] private TrailResetHandler _trailResetHandler;
    [SerializeField] private SpiderWebHandler _spiderWeb;
    
    public void Initialize()
    {
        _preAttackFlash.Initialize();
        _damageFlash.Initialize();
        _deathFlash.Initialize();
        _healthIndication.Initialize();
        _spiderWeb.Initialize();
        
        _movement.PreAttackStarted += OnPreAttackStarted;
        _movement.PreAttackEnded += OnPreAttackEnded;
        _spider.Started += OnSpiderStarted;
        _spider.Damaged += OnSpiderDamaged;
        _spider.HealthChanged += _healthIndication.Refresh;
        _spider.Dead += OnSpiderDead;
        
    }
    
    private void OnDestroy()
    {
        _movement.PreAttackStarted -= OnPreAttackStarted;
        _movement.PreAttackEnded -= OnPreAttackEnded;
        _spider.Started -= OnSpiderStarted;
        _spider.Damaged -= OnSpiderDamaged;
        _spider.HealthChanged -= _healthIndication.Refresh;
        _spider.Dead -= OnSpiderDead;
    }
    
    private void OnSpiderStarted()
    {
        _trailResetHandler.Initialize();
        _deathFlash.Initialize();
        _spiderWeb.Play();
    }

    private void OnPreAttackStarted()
    {
        _preAttackFlash.PreAttackStart();
    }

    private void OnPreAttackEnded()
    {
        _preAttackFlash.PreAttackEnd();
    }

    private void OnSpiderDamaged()
    {
        _damageFlash.Play();
    }

    private void OnSpiderDead()
    {
        _deathFlash.Play();
        _spiderWeb.StartShrink(true);
    }
}
