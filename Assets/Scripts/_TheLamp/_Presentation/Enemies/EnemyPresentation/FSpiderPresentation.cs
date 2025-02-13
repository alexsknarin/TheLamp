using UnityEngine;

public class FSpiderPresentation: MonoBehaviour
{
    [SerializeField] private FSpider _fSpider;
    [SerializeField] private FSpiderMovement _movement;
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
        _fSpider.Started += OnSpiderStarted;
        _fSpider.Damaged += OnSpiderDamaged;
        _fSpider.HealthChanged += _healthIndication.Refresh;
        _fSpider.Dead += OnSpiderDead;
        
    }
    
    private void OnDestroy()
    {
        _movement.PreAttackStarted -= OnPreAttackStarted;
        _movement.PreAttackEnded -= OnPreAttackEnded;
        _fSpider.Started -= OnSpiderStarted;
        _fSpider.Damaged -= OnSpiderDamaged;
        _fSpider.HealthChanged -= _healthIndication.Refresh;
        _fSpider.Dead -= OnSpiderDead;
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
