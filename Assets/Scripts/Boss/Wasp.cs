using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wasp : MonoBehaviour
{
    [SerializeField] private WaspMovement _waspMovement;
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _currentHealth;
    public bool ReadyToLampDamage { get; private set; }

    private void OnEnable()
    {
        LampAttackModel.OnLampAttack += LampAttack;
    }

    private void LampAttack(int attackPower, float currentPower, float attackDuration, float attackDistance)
    {
        Debug.Log("LampAttack");
        if (ReadyToLampDamage)
        {
            Debug.Log("ReadyToLampDamage");
            ReceiveDamage(attackPower);
        }
    }
    
    private void Start()
    {
        _currentHealth = _maxHealth;
        _waspMovement.Initialize();
        _waspMovement.Play();
    }
    
    public void ReceiveDamage(int damage)
    {
        Debug.Log("ReceiveDamage " + damage.ToString());
        _currentHealth -= damage;
        // if (_currentHealth > 0)
        // {
        // }
        // else
        // {
        // }    
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
