using UnityEngine;

public class LadybugPresentation : EnemyPresentation
{
    [SerializeField] private PreAttackFlash _preAttackFlash;
    // [SerializeField] private DamageFlash _damageFlash;
    [SerializeField] private LadybugDamageFlash _damageFlash;
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
        _damageFlash.Perform();
    }
    
    public override void DeathFlash()
    {
        _deathFlash.Perform();
    }
    
    public override void Initialize()
    {
        IInitializable _preAttackFlashInit = _preAttackFlash;
        IInitializable _damageFlashInit = _damageFlash;
        IInitializable _deathFlashInit = _deathFlash;
        _preAttackFlashInit.Initialize();
        _damageFlashInit.Initialize();
        _deathFlashInit.Initialize();
    }
}
