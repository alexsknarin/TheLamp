using UnityEngine;

public class DragonflyPresentation : EnemyPresentation
{
    [SerializeField] private DragonflyDamageFlash _damageIndication;
    [SerializeField] private DragonflyHealthIndication _healthIndication;
    [SerializeField] private DragonflyDeathFlash _deathFlash;
    [SerializeField] private DragonflyPreAttackFlash _preAttackFlash;
    [SerializeField] private DragonflySwarmCallPresentation _swarmCallPresentation;

    
    // private bool _isDamageFlashing = false;
    private float _localTime = 0f;

    public override void PreAttackStart()
    {
        _preAttackFlash.PreAttackStart();
    }

    public override void PreAttackEnd()
    {
        _preAttackFlash.PreAttackEnd();
    }
    
    public void SetActiveColliderTransform(Transform transform)
    {
        _damageIndication.SetContactCollisionTransform(transform);
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

    public void SwarmCall()
    {
        _swarmCallPresentation.Play();
    }

    public override void Initialize()
    {
        _damageIndication.Initialize();
        _healthIndication.Initialize();
        _deathFlash.Initialize();
        _preAttackFlash.Initialize();
        _swarmCallPresentation.Initialize();
    }
    
    
}
