using UnityEngine;

public class FlyPresentation : EnemyPresentation
{
    [SerializeField] private PreAttackFlash _preAttackFlash;
    [SerializeField] private DamageIndication _damageFlash;
    [SerializeField] private DamageIndication _deathFlash;
    [SerializeField] private HealthIndication _healthIndication;
    
   
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
        _damageFlash.StartPerform();
    }
    
    public override void DeathFlash()
    {
        _deathFlash.StartPerform();
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
    }
}
