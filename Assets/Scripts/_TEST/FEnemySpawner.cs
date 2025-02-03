using System.Collections.Generic;
using UnityEngine;

public class FEnemySpawner: ITickable, IInitializable, IDisposable
{
    // Dependencies
    private FEnemyFactory _enemyFactory;
    
    private EnemyQueue _enemyQueue;
    private FEnemyPool _enemyPool;
    private List<FEnemy> _enemies;
    
    private float _localTime;
    private bool _isWaveActive = false;
    private int _currentEnemyIndex;


    public FEnemySpawner(FEnemyFactory enemyFactory)
    {
        _enemyFactory = enemyFactory;
    }

    public void Initialize()
    {
        _enemyPool = new FEnemyPool(_enemyFactory);
        _enemyPool.Initialize();
        _enemyPool.EnemyReleased += OnEnemyReleased;
    }

    public void Dispose()
    {
        _enemyPool.EnemyReleased -= OnEnemyReleased;
    }

    private void OnEnemyReleased(FEnemy enemy)
    {
        _enemies.Remove(enemy);
    }

    public void PrepareWave(EnemyQueue enemyQueue, List<FEnemy> enemies)
    {
        _enemyQueue = enemyQueue;
        
        // TODO: redo this to use types and dictionary ?
        for (int i = 0; i < _enemyQueue.Count(); i++)
        {
            EnemyType enemyType = _enemyQueue.Get(i);
            if (enemyType == EnemyType.Mothling)
            {
                _enemyPool.PreloadEnemy(typeof(FMothling));                
            }
        }
        
        _enemies = enemies;
        _enemies.Clear();
    }

    public void StartWave()
    {
        _localTime = 0;
        _isWaveActive = true;
        _currentEnemyIndex = 0;
    }


    public void Tick(float deltaTime)
    {
        if (_isWaveActive && _currentEnemyIndex < _enemyQueue.Count())
        {
            if (_localTime >= 6f)
            {
                _localTime = 0;
                
                if (_enemyQueue.Get(_currentEnemyIndex) == EnemyType.Mothling)
                {
                    FEnemy enemy = _enemyPool.Get(typeof(FMothling));
                    _enemies.Add(enemy);
                    enemy.Play();
                    _currentEnemyIndex++;
                }
            }
            
            _localTime += deltaTime;
        }
        
        
    }
}
