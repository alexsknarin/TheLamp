using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class Lamp : MonoBehaviour, IInitializable
{
    [SerializeField] private LampAttackModel _lampAttackModel;
    [SerializeField] private LampPresentation _lampPresentation;
    [SerializeField] private LampCollisionHandler _lampCollisionHandler;
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _currentHealth;
    private bool _isAttackSuccess = false;
    
    public static event Action OnLampDamaged;
    
    // 
    private float _damageAssessmentDuration = 0.1f;
    private float _prevTime;
    private bool _isAssessingDamage = false;

    private void OnEnable()
    {
        _lampCollisionHandler.OnLampCollidedEnemy += AssessDamage;
        Enemy.OnEnemyDamaged += AttackSuccessConfirm;
    }
    
    private void OnDisable()
    {
        _lampCollisionHandler.OnLampCollidedEnemy -= AssessDamage;
        Enemy.OnEnemyDamaged += AttackSuccessConfirm;
    }

    public void Initialize()
    {
        _lampAttackModel.Initialize();
        _currentHealth = _maxHealth;
    }

    private void AttackSuccessConfirm()
    {
        _isAttackSuccess = true;
    }

    // REMAKE with async or coroutine
    private void AssessDamage()
    {
        if (!_isAssessingDamage)
        {
            _prevTime = Time.time;
            _isAssessingDamage = true;
        }
    }
    
    public void HandleDamage()
    {
        if (_isAssessingDamage)
        {
            float assessmentPhase = (Time.time - _prevTime) / _damageAssessmentDuration;
            if (assessmentPhase > 1)
            {
                if (_isAttackSuccess)
                {
                    _isAttackSuccess = false;
                    _isAssessingDamage = false;
                }
                else
                {
                    _isAttackSuccess = false;
                    _isAssessingDamage = false;
                    _currentHealth--;
                    _lampPresentation.StartDamageState();
                    OnLampDamaged?.Invoke();
                    if (_currentHealth <= 0)
                    {
                    }
                }
            }    
        }
    }

    private void Update()
    {
        HandleDamage();
    }
}
