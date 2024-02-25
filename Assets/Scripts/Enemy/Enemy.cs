using System;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : MonoBehaviour, IInitializable
{
    [SerializeField] private int _maxHealth;
    [SerializeField] private EnemyMovement _enemyMovement;
    [SerializeField] private EnemyPresentation _enemyPresentation;

    private void OnEnable()
    {
        _enemyMovement.OnPreAttackStart += _enemyPresentation.PreAttackStart;
        _enemyMovement.OnPreAttackEnd += _enemyPresentation.PreAttackEnd;   
    }
    
    private void OnDisable()
    {
        _enemyMovement.OnPreAttackStart += _enemyPresentation.PreAttackStart;
        _enemyMovement.OnPreAttackEnd += _enemyPresentation.PreAttackEnd;
    }
    
    public void Attack()
    {
        _enemyMovement.TriggerAttack();
    }
    
    public void Initialize()
    {
        _enemyMovement.Initialize();
    }
}
