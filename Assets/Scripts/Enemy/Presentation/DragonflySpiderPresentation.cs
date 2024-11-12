using UnityEngine;

public class DragonflySpiderPresentation : EnemyPresentation
{
    [SerializeField] private DeathFlash _deathFlash; 
    [Header("------ Preattack Flash ------")]
    [SerializeField] private PreAttackFlash _preAttackFlash;
    [SerializeField] private DragonflySpiderWebHandler _spiderWeb;
    
    public override void Initialize()
    {
        _deathFlash.Initialize();
        _preAttackFlash.Initialize();  // TODO: remove null check later
        _spiderWeb.Initialize();
    }
    
    public void Play()
    {
        _spiderWeb?.Play(transform);
        _deathFlash.Initialize();
    }
    
    public void SwitchToCaughtState()
    {
        _spiderWeb.StartShrink();
    }
    
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
