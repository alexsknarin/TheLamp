using System;
using UnityEngine;

public class Wasp : BossBase
{
    [SerializeField] private WaspMovement _waspMovement;
    [SerializeField] private WaspPresentation _waspPresentation;
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _currentHealth;
    public override EnemyType EnemyType => EnemyType.Wasp;

    private void OnEnable()
    {
        _waspMovement.OnBossAttackStartedEvent += UpdateRecievedLampAttackStatus;
        _waspMovement.OnDeathStateEndedEvent += HandleDeathMoveStateEnd;
        _waspMovement.OnLeftTheScreenEvent += HandleLeftScreen;
        Lamp.OnLampDeadEvent += HandleLampDead; // TODO: manage from enemy manager
    }
    
    private void OnDisable()
    {
        _waspMovement.OnBossAttackStartedEvent -= UpdateRecievedLampAttackStatus;
        _waspMovement.OnDeathStateEndedEvent -= HandleDeathMoveStateEnd;
        _waspMovement.OnLeftTheScreenEvent -= HandleLeftScreen;
        Lamp.OnLampDeadEvent -= HandleLampDead;
    }
    public override void Initialize()
    {
        ReceivedLampAttack = false;
        IsGameOver = false;
        _currentHealth = _maxHealth;
        _waspPresentation.Initialize();
        _waspMovement.Initialize();
    }

    public override void Reset()
    {
        ReceivedLampAttack = false;
        _currentHealth = _maxHealth;
        _waspPresentation.Initialize();
        _waspMovement.MovementReset();
    }
    
    public override void Play()
    {
        _waspMovement.Play();
        _waspPresentation.ResetTrail();
        _waspPresentation.Initialize();
    }

    public void TriggerSpread()
    {
        OnTriggerSpreadInvoke();
    }
    
    private void UpdateRecievedLampAttackStatus()
    {
        if (ReceivedLampAttack)
        {
            ReceivedLampAttack = false;
        }
    }

    public override void HandleCollisionWithStickZone()
    {
    }   

    public override void ReceiveDamage(int damage)
    {
        _currentHealth -= damage;
        ReceivedLampAttack = true;
        if (_currentHealth > 0)
        {
            float damagePhase = (1 - (float)_currentHealth/_maxHealth) + 0.2f;
            damagePhase = Mathf.Clamp(damagePhase, 0, 1);
            _waspPresentation.SetDamage(damagePhase);
            _waspPresentation.PlayDamageParticles();
        }
        else
        {
            _waspMovement.SetDead();
            _waspPresentation.PlayDeath();
            _waspPresentation.PlayDamageParticles();
        }    
    }

    public override void UpdateAttackAvailability()
    {
    }

    public override void ReturnToPool()
    {
    }

    public override void SpreadStart()
    {
    }

    public override void StartAttack()
    {
    }
    
    public override void HandleCollisionWithLamp()
    {
    }
    
    public override Vector3 ProvideImpactPoint()
    {
        return transform.position;
    }

    private void HandleLeftScreen()
    {
        if (IsGameOver)
        {
            Reset();
            IsGameOver = false;
        }
    }
    
    private void HandleLampDead(EnemyBase enemy)
    {
        _waspMovement.SetLampDestroyed();
    }
    
    private void HandleDeathMoveStateEnd()
    {
        OnDeathInvoke();
        _waspPresentation.Reset();
    }

    private void ResetTrail()
    {
        _waspPresentation.ResetTrail();
    }
}
