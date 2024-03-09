using UnityEngine;

public class MothPresentation : EnemyPresentation
{
    [SerializeField] private PreAttackFlash _preAttackFlash;
    [SerializeField] private DamageFlash _damageFlash;
    [SerializeField] private DeathFlash _deathFlash;

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
    
    public override void Initialize()
    {
        _preAttackFlash.Initialize();
        _damageFlash.Initialize();
        _deathFlash.Initialize();
    }
}