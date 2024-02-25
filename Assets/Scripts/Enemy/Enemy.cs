using System;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour, IInitializable
{
    [SerializeField] private int _maxHealth;
    [SerializeField] private EnemyMovement _enemyMovement;
    [SerializeField] private EnemyPresentation _enemyPresentation;
    [SerializeField] private EnemyCollisionHandler _enemyCollisionHandler;

    private void OnEnable()
    {
        _enemyMovement.OnPreAttackStart += _enemyPresentation.PreAttackStart;
        _enemyMovement.OnPreAttackEnd += _enemyPresentation.PreAttackEnd;
        _enemyCollisionHandler.OnCollidedWithLamp += _enemyMovement.TriggerFall;
    }
    
    private void OnDisable()
    {
        _enemyMovement.OnPreAttackStart += _enemyPresentation.PreAttackStart;
        _enemyMovement.OnPreAttackEnd += _enemyPresentation.PreAttackEnd;
        _enemyCollisionHandler.OnCollidedWithLamp -= _enemyMovement.TriggerFall;
    }
    
    public void Attack()
    {
        _enemyMovement.TriggerAttack();    
    }
    
    public void Initialize()
    {
        _enemyMovement.Initialize();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.LeftControl))
        {
            _enemyMovement.TriggerDeath();
        }
    }
}
