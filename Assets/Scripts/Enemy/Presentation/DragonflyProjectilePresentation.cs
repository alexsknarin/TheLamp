using UnityEngine;

public class DragonflyProjectilePresentation : EnemyPresentation
{
    [SerializeField] private DeathFlash _deathFlash; 
    [Header("------ Preattack Flash ------")]
    [SerializeField] private PreAttackFlash _preAttackFlash;
    
    
    public override void Initialize()
    {
        _deathFlash.Initialize();
        _preAttackFlash?.Initialize();  // TODO: remove null check later
    }
    
    public override void PreAttackStart()
    {
        _preAttackFlash?.PreAttackStart();
    }

    public override void PreAttackEnd()
    {
        _preAttackFlash?.PreAttackEnd();
    }

    public override void DamageFlash()
    {
        throw new System.NotImplementedException();
    }

    public override void DeathFlash()
    {
        _deathFlash.Play();
    }

    public override void HealthUpdate(int currentHealth, int maxHealth)
    {
        throw new System.NotImplementedException();
    }


}
