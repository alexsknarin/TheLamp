using System.Collections.Generic;
using UnityEngine;

public class DragonflyPresentation : EnemyPresentation
{
    [SerializeField] private DamageIndication _damageIndication;
    [SerializeField] private DragonflyHealthIndication _healthIndication;
    [SerializeField] private DragonflyDeathFlash _deathFlash;

    
    // private bool _isDamageFlashing = false;
    private float _localTime = 0f;
    
    
    
    public override void PreAttackStart()
    {
    }

    public override void PreAttackEnd()
    {
    }

    public override void DamageFlash()
    {
        // _isDamageFlashing = true;
        // _localTime = 0f;
        _damageIndication.Play();
    }

    public override void DeathFlash()
    {
        _deathFlash.Play();
    }

    public override void HealthUpdate(int currentHealth, int maxHealth)
    {
        _healthIndication.Refresh(currentHealth, maxHealth);
    }

    public override void Initialize()
    {
        _damageIndication.Initialize();
        _healthIndication.Initialize();
        _deathFlash.Initialize();
    }
}
