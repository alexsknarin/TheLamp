using System;
using UnityEngine;

public class Wasp : EnemyBase
{
    [SerializeField] private WaspMovement _waspMovement;
    [SerializeField] private WaspPresentation _waspPresentation;
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _currentHealth;
    
    public event Action OnTriggerSpread; 


    private void OnEnable()
    {
        LampAttackModel.OnLampAttack += LampAttack;
        _waspMovement.OnWaspAttackStarted += UpdateRecievedLampAttackStatus;
    }
    
    private void OnDisable()
    {
        LampAttackModel.OnLampAttack -= LampAttack;
        _waspMovement.OnWaspAttackStarted -= UpdateRecievedLampAttackStatus;
    }
    public virtual void Initialize()
    {
        ReceivedLampAttack = false;
        _currentHealth = _maxHealth;
        _waspPresentation.Initialize();
        _waspMovement.Initialize();
    }

    public void Reset()
    {
        ReceivedLampAttack = false;
        _currentHealth = _maxHealth;
        _waspPresentation.Initialize();
        _waspMovement.MovementReset();
    }

    public void Play()
    {
        _waspMovement.Play();
    }
    
    private void LampAttack(int attackPower, float currentPower, float attackDuration, float attackDistance)
    {
        if (ReadyToLampDamage)
        {
            ReceiveDamage(attackPower);
            _waspMovement.SetDamaged();
        }
    }
    
    public void TriggerSpread()
    {
        OnTriggerSpread?.Invoke();
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
        }
        else
        {
            _waspMovement.SetDead();
            _waspPresentation.PlayDeath();
        }    
    }

    public override void UpdateAttackAvailability()
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
     //   
    }
    
    public override void HandleExitingAttackExitZone()
    {
        ReadyToLampDamage = false;
    }
    
}
