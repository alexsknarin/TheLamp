using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

// TODO: find out how to stop the wave
// TODO: implement WaveEnded event
public class FEnemyAttacker: ITickable
{
    // Dependencies
    private readonly float _maxAggressionLevel;
    private readonly LampCollisionDetectionService _lampCollisionDetectionService;
    
    private float _attackCooldown;
    private float _aggressionLevelNormalized;
    private bool _isWaveActive = false;
    private float _localTime;
    private bool _isCooldownActive = false;

    private List<FEnemy> _enemies;
    private List<CollidableEnemy> _enemiesReadyToAttack = new ();

    public FEnemyAttacker(float maxAggressionLevel, LampCollisionDetectionService lampCollisionDetectionService)
    {
        _maxAggressionLevel = maxAggressionLevel;
        _lampCollisionDetectionService = lampCollisionDetectionService;
    }
    
    
    
    // public event Action<CollidableEnemy> EnemyAttackStarted; // Provide the enemy that started the attack
    
    public void PrepareWave(int aggressionLevel, List<FEnemy> enemies)
    {
        _enemies = enemies;
        _enemiesReadyToAttack.Clear();
        _aggressionLevelNormalized = aggressionLevel / _maxAggressionLevel;
    }
    
    public void StartWave()
    {
        // TODO: fix magic numbers
        _attackCooldown = GetRandomAttackDelay(4.5f, 1.8f, 6.5f, 2.8f, _aggressionLevelNormalized);
        _isWaveActive = true;
        _isCooldownActive = true;
        _localTime = 0;
    }
    
    public void StopWave()
    {
        _isWaveActive = false;
    }
    
    public void BlockAttackCooldown()
    {
        Debug.Log("Cooldown blocked");
        _isCooldownActive = false;
    }
    
    public void UnblockAttackCooldown()
    {
        Debug.Log("Cooldown unblocked");
        _isCooldownActive = true;
    }
    
    public void Tick(float deltaTime)
    {
        if (_isWaveActive)
        {
            UpdateEnemiesReadyToAttack();
            WaitForCooldown();    
        }
    }
    
    private void UpdateEnemiesReadyToAttack()
    {
        _enemiesReadyToAttack.Clear();
       
        foreach (var enemy in _enemies)
        {
            if (enemy is CollidableEnemy && enemy.IsReadyToAttack)
            {
                _enemiesReadyToAttack.Add((CollidableEnemy)enemy);
            }
        }
    }
    
    private void WaitForCooldown()
    {
        if (_localTime >= _attackCooldown)
        {
            _localTime = 0;
            Attack();
        }
        else
        {
            if (_isCooldownActive)
            {
                // TODO: add exception for Megamothling and Megabeetle (BOSSES)
                _localTime += Time.deltaTime;   
            }
        }
    }
    
    private void Attack()
    {
        // Select random Enemy from available
        int enemyIndex;
        if (_enemiesReadyToAttack.Count > 0)
        {
            enemyIndex = Random.Range(0, _enemiesReadyToAttack.Count);    
        }
        else
        {
            return;
        }
        
        var attackingEnemy = _enemiesReadyToAttack[enemyIndex];
        

        // MEGAMOTHLING: TODO: add later
        // It attacks alongside other enemies, but it should be
        // Prioritized to attack more often
        // In this case twice as often
        
        // if (_isBossActive && (_boss.EnemyType == EnemyType.Megamothling) && _boss.ReadyToAttack)
        // {
        //     int megamothlingAttackChance = Random.Range(0, 2);
        //     if (megamothlingAttackChance == 0)
        //     {
        //         attackingEnemy = _boss;
        //     }
        // }
       
        // TODO: add tp damageables
        _lampCollisionDetectionService.AddCollidable(attackingEnemy);
        attackingEnemy.Attack();
        
        _localTime = 0;
        // TODO: fix magic numbers
        _attackCooldown = GetRandomAttackDelay(2.5f, 0.8f, 6.1f, 1.8f, _aggressionLevelNormalized); 
    }
    
    private float GetRandomAttackDelay(float minMin, float minMax, float maxMin, float maxMax, float aggressionLevel)
    {
        return Random.Range(
            Mathf.Lerp(minMin, minMax, aggressionLevel),
            Mathf.Lerp(maxMin, maxMax, aggressionLevel)
        );
    }

}
