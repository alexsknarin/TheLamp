using System;
using UnityEngine;

public class Wasp : BossBase
{
    public override EnemyTypes EnemyType => EnemyTypes.Wasp;
    [SerializeField] private WaspMovement _waspMovement;
    [SerializeField] private WaspPresentation _waspPresentation;
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _currentHealth;


    private void OnEnable()
    {
        _waspMovement.OnWaspAttackStarted += UpdateRecievedLampAttackStatus;
        _waspMovement.OnDeathStateEnded += HandleDeathMoveStateEnd;
        _waspMovement.OnLeftTheScreen += HandleLeftScreen;
    }
    
    private void OnDisable()
    {
        _waspMovement.OnWaspAttackStarted -= UpdateRecievedLampAttackStatus;
        _waspMovement.OnDeathStateEnded -= HandleDeathMoveStateEnd;
        _waspMovement.OnLeftTheScreen -= HandleLeftScreen;
    }
    public override void Initialize()
    {
        gameObject.SetActive(true);
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

    public override void AttackStart()
    {
    }

    public override void HandleEnteringAttackZone()
    {
        ReadyToLampDamage = true;
    }
    
    public override void HandleCollisionWithLamp()
    {
    }
    
    public override void HandleExitingAttackExitZone()
    {
        ReadyToLampDamage = false;
    }

    private void HandleLeftScreen()
    {
        if (IsGameOver)
        {
            Reset();
            gameObject.SetActive(false);
            IsGameOver = false;
        }
    }
    
    private void HandleDeathMoveStateEnd()
    {
        OnDeathInvoke();
    }

    private void ResetTrail()
    {
        _waspPresentation.ResetTrail();
    }
}
