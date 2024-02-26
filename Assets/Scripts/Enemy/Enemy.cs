using System;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour, IInitializable
{
    [SerializeField] private int _maxHealth;
    [SerializeField] private int _currentHealth;
    [SerializeField] private EnemyMovement _enemyMovement;
    [SerializeField] private EnemyPresentation _enemyPresentation;
    [SerializeField] private EnemyCollisionHandler _enemyCollisionHandler;

    private void OnEnable()
    {
        _enemyMovement.OnPreAttackStart += PreAttack;
        _enemyMovement.OnPreAttackEnd += _enemyPresentation.PreAttackEnd;
        _enemyCollisionHandler.OnCollidedWithLamp += Fall;
    }
    
    private void OnDisable()
    {
        _enemyMovement.OnPreAttackStart += PreAttack;
        _enemyMovement.OnPreAttackEnd += _enemyPresentation.PreAttackEnd;
        _enemyCollisionHandler.OnCollidedWithLamp -= Fall;
    }
    
    public void Attack()
    {
        _enemyMovement.TriggerAttack();
    }
    
    private void PreAttack()
    {
        _enemyCollisionHandler.EnableCollider();        
        _enemyPresentation.PreAttackStart();
    }
    
    private void Fall()
    {
        _enemyCollisionHandler.DisableCollider();
        _enemyMovement.TriggerFall();
    }
    
    public void RecieveDamage(int damage)
    {
        _currentHealth -= damage;
        if (_currentHealth > 0)
        {
            _enemyPresentation.DamageFlash();
        }
        else
        {
            _currentHealth = 0;
            _enemyMovement.TriggerDeath();
            _enemyPresentation.DeathFlash();
        }
    }
    
    public void Initialize()
    {
        _enemyMovement.Initialize();
        _currentHealth = _maxHealth;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            _enemyMovement.TriggerDeath();
        }
    }
}
