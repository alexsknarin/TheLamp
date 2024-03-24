using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wasp : MonoBehaviour
{
    [SerializeField] private WaspMovement _waspMovement;
    [SerializeField] private WaspPresentation _waspPresentation;
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _currentHealth;
    public bool ReadyToLampDamage { get; private set; }

    private void OnEnable()
    {
        LampAttackModel.OnLampAttack += LampAttack;
    }

    private void LampAttack(int attackPower, float currentPower, float attackDuration, float attackDistance)
    {
        if (ReadyToLampDamage)
        {
            ReceiveDamage(attackPower);
            _waspMovement.SetDamaged();
        }
    }
    
    private void Start()
    {
        _currentHealth = _maxHealth;
        _waspPresentation.Initialize();
        _waspMovement.Initialize();
        _waspMovement.Play();
    }
    
    public void ReceiveDamage(int damage)
    {
        _currentHealth -= damage;
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
    
    public void HandleEnteringAttackZone()
    {
        ReadyToLampDamage = true;
    }
    
    public void HandleCollisionWithLamp()
    {
     //   
    }
    
    public void HandleExitingAttackExitZone()
    {
        ReadyToLampDamage = false;
    }
    
}
