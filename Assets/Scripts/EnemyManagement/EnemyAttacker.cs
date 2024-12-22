using System.Collections.Generic;
using UnityEngine;

public class EnemyAttacker
{
    public bool IsBossActive => _isBossActive;
    
    // Dependencies
    private readonly SpawnQueue _spawnQueue;
    private List<EnemyBase> _enemies;
    List<EnemyBase> _enemiesReadyToAttack;
    List<EnemyBase> _ladybugsPatrolling;
    private float _maxAggressionLevel;
    
    private EnemyQueue _enemyQueue;
    
    private float _attackCooldown;
    private float _agressionLevelNormalized;
    private float _localTime;
    private bool _isWaveActive = false;
    private bool _isBossActive = false;
    private BossBase _boss;

    public EnemyAttacker(
        SpawnQueue spawnQueue, 
        List<EnemyBase> enemies, 
        List<EnemyBase> enemiesReadyToAttack, 
        List<EnemyBase> ladybugsPatrolling, 
        float maxAggressionLevel
        )
    {
        _spawnQueue = spawnQueue;
        _enemies = enemies;
        _enemiesReadyToAttack = enemiesReadyToAttack;
        _ladybugsPatrolling = ladybugsPatrolling;
        _maxAggressionLevel = maxAggressionLevel;
    }
    
    public void StartWave(int waveIndex)
    {
        _enemyQueue = _spawnQueue.Get(waveIndex);
        
        _agressionLevelNormalized = _enemyQueue.AggressionLevel / _maxAggressionLevel;
        _attackCooldown = GetRandomAttackDelay(4.5f, 1.8f, 6.5f, 2.8f, _agressionLevelNormalized); // TODO: fix magic numbers
        
        _isBossActive = false;
        _localTime = 0;
        _isWaveActive = true;
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
            // Update time only if no Ladybug is close to the enemies
            // and if Boss is not active
            // unless it's Megamothling or Megabeetle
            if (CheckIfAttackTimeUpdateIsAllowed() && 
                (!_isBossActive || (_boss.EnemyType == EnemyType.Megamothling || _boss.EnemyType == EnemyType.Megabeetle))) 
            {
                _localTime += Time.deltaTime;
            }
        }
    }

    private void Attack()
    {
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
        

        // MEGAMOTHLING:
        // It attacks alongside other enemies, but it should be
        // Prioritized to attack more often
        // In this case twice as often
        
        if (_isBossActive && (_boss.EnemyType == EnemyType.Megamothling) && _boss.ReadyToAttack)
        {
            int megamothlingAttackChance = Random.Range(0, 2);
            if (megamothlingAttackChance == 0)
            {
                attackingEnemy = _boss;
            }
        }
       
        attackingEnemy.StartAttack();
        
        _localTime = 0;
        _attackCooldown = GetRandomAttackDelay(2.5f, 0.8f, 6.1f, 1.8f, _agressionLevelNormalized); // TODO: fix magic numbers
    }

    public void ActivateBoss(BossBase boss)
    {
        _boss = boss;
        _localTime = 0;
        _isBossActive = true;
    }
    
    public void DeactivateBoss()
    {
        _isBossActive = false;
    }
    
    private void UpdateEnemiesReadyToAttack()
    {
        _enemiesReadyToAttack.Clear();
        foreach (var enemy in _enemies)
        {
            enemy.UpdateAttackAvailability();
            if (enemy.ReadyToAttack)
            {
                _enemiesReadyToAttack.Add(enemy);
            }
        }
    }
    
    private bool CheckIfAttackTimeUpdateIsAllowed()
    {
        if (_ladybugsPatrolling.Count > 0)
        {
            foreach (var ladybug in _ladybugsPatrolling)
            {
                Vector3 pos = ladybug.transform.position;
                pos.z = 0;
                // if any Ladybug is at 0.87f distance or close - no enemy attack is allowed
                // TODO: need to get the dependency to the lamp's actual position
                // TODO: fix magic number - extract to a config
                if (pos.magnitude < 0.87f || ladybug.IsAttacking)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private float GetRandomAttackDelay(float minMin, float minMax, float maxMin, float maxMax, float aggressionLevel)
    {
        return Random.Range(
            Mathf.Lerp(minMin, minMax, aggressionLevel),
            Mathf.Lerp(maxMin, maxMax, aggressionLevel)
        );
    }

    public void Tick()
    {
        UpdateEnemiesReadyToAttack();
        WaitForCooldown();
    }
}
