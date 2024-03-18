using UnityEngine;

public class SpiderPresentation : EnemyPresentation
{
    [SerializeField] private PreAttackFlash _preAttackFlash;
    [SerializeField] private DamageFlash _damageFlash;
    [SerializeField] private DeathFlash _deathFlash;
    [SerializeField] private HealthIndication _healthIndication;
    [SerializeField] private SpiderWebHandler _spiderWeb;
    

    public override void PreAttackStart()
    {
        _preAttackFlash.PreAttackStart();
    }
    
    public override void PreAttackEnd()
    {
        _preAttackFlash.PreAttackEnd();
    }
    
    public override void DamageFlash()
    {
        _damageFlash.Play();
    }
    
    public override void DeathFlash()
    {
        _deathFlash.Play();
        _spiderWeb.StartShrink();
    }

    public override void HealthUpdate(int currentHealth, int maxHealth)
    {
        _healthIndication.Refresh(currentHealth, maxHealth);
    }

    public override void Initialize()
    {
        _preAttackFlash.Initialize();
        _damageFlash.Initialize();
        _deathFlash.Initialize();
        _healthIndication.Initialize();
        _spiderWeb.Initialize();
    }
}
