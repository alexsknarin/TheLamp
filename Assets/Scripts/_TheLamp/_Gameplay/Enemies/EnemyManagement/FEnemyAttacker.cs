using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class FEnemyAttacker: ITickable
{
    private float _maxAggressionLevel;
    
    private float _attackCooldown;
    private float _aggressionLevelNormalized;
    private float _localTime;
    private bool _isWaveActive = false;
    
    private List<FEnemy> _enemies;
    private List<FEnemy> _enemiesReadyToAttack;
    
    public event Action<CollidableEnemy> EnemyAttackStarted;
    
    
    public void PrepareWave(int aggressionLevel, List<FEnemy> enemies)
    {
        _enemies = enemies;
        _enemiesReadyToAttack = new List<FEnemy>();
        _aggressionLevelNormalized = aggressionLevel / _maxAggressionLevel;
    }
    
    public void StartWave()
    {
        _attackCooldown = 1f;
        _isWaveActive = true;
        _localTime = 0;
    }

    
    public void Tick(float deltaTime)
    {
        if (_isWaveActive)
        {
            UpdateEnemiesReadyToAttack();
            
            if( _localTime >= _attackCooldown)
            {
                Attack();
            }

            _localTime += deltaTime;
        }
    }
    
    private void UpdateEnemiesReadyToAttack()
    {
        _enemiesReadyToAttack.Clear();
        
        if (_enemies.Count == 0)
        {
            return;
        }
        
        foreach (var enemy in _enemies)
        {
            if (enemy.IsReadyToAttack)
            {
                _enemiesReadyToAttack.Add(enemy);
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
        attackingEnemy.Attack();
        EnemyAttackStarted?.Invoke((CollidableEnemy)attackingEnemy);
        _localTime = 0;
    }

}
