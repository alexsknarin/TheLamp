using System;
using System.Collections.Generic;
// using System.Linq;
using UnityEngine;

public class EnemySpawner : ITickable
{
    private readonly EnemyType[] BOSS_TYPES = new EnemyType[]
    {
        EnemyType.Wasp,
        EnemyType.Megamothling,
        EnemyType.Megabeetle,
        EnemyType.Dragonfly
    };
    
    // Dependencies
    private readonly SpawnQueue _spawnQueue;
    private List<EnemyBase> _enemies; // TODO: replace with actual enemy objects from Enemy Manager
    private IEnemyPool _enemyPool;
    private float _firstEnemySpawnDelay;
    // Bosses
    private BossBase _waspBoss;
    private BossBase _megamothlingBoss;
    private BossBase _megabeetleBoss;
    private BossBase _dragonflyBoss;

    private EnemyQueue _enemyQueue;
    private int _currentWave;
    private int _currentEnemyIndex;
    private float _spawnCooldown;
    private float _localTime;
    private bool _isWaveActive = false;
    
    public EnemySpawner(
        SpawnQueue spawnQueue, 
        List<EnemyBase> enemies,
        IEnemyPool enemyPool,
        BossBase waspBoss,
        BossBase megamothlingBoss,
        BossBase megabeetleBoss,
        BossBase dragonflyBoss,
        float firstEnemySpawnDelay
        )
    {
        _spawnQueue = spawnQueue;
        _enemies = enemies;
        _enemyPool = enemyPool;
        _waspBoss = waspBoss;
        _megamothlingBoss = megamothlingBoss;
        _megabeetleBoss = megabeetleBoss;
        _dragonflyBoss = dragonflyBoss;
        _firstEnemySpawnDelay = firstEnemySpawnDelay;
    }
    
    public event Action<BossBase> BossSpawned;
    
    public int EnemiesWaveCount { get; private set; }
    public int EnemiesAvailable { get; private set; }
    public int MaxEnemiesOnScreen { get; private set; }

    public BossBase Boss { get; private set; }
    
    public void StartWave(int waveIndex)
    {
        _currentWave = waveIndex;
        _enemyQueue = _spawnQueue.Get(waveIndex);
        
        // Debug info
        // Debug.Log("---------------------");
        // Debug.Log("Starting wave " + waveIndex);
        // Debug.Log("Number of enemies in the wave: " + _enemyQueue.Count());
        // Debug.Log("Max enemies on screen: " + _enemyQueue.MaxEnemiesOnScreen);
        // Debug.Log("Aggression level: " + _enemyQueue.AggressionLevel);
        // Debug.Log("Spawn delay: " + _enemyQueue.SpawnDelay);
        // Debug.Log("Spawn delay acceleration: " + _enemyQueue.SpawnDelayAcceleration);
        // Debug.Log("- Enemy types -");
        // for (int i = 0; i < _enemyQueue.Count(); i++)
        // {
        //     Debug.Log(_enemyQueue.Get(i));
        // }
        // Debug.Log("First enemy spawn delay: " + _firstEnemySpawnDelay);
        // Debug.Log("---------------------");
        
        // Start spawning enemies
        EnemiesWaveCount = _enemyQueue.Count();
        EnemiesAvailable = _enemyQueue.Count();
        MaxEnemiesOnScreen = _enemyQueue.MaxEnemiesOnScreen;
        
        _spawnCooldown = _firstEnemySpawnDelay;
        _localTime = 0;
        _currentEnemyIndex = 0;
        _isWaveActive = true;
        
    }

    public void Tick(float deltaTime)
    {
        if (_isWaveActive)
        {
            WaitForCooldown(deltaTime);
        }
    }

    private void WaitForCooldown(float deltaTime)
    {
        if (_localTime >= _spawnCooldown)
        {
            _localTime = 0;
            SpawnEnemies();
            _spawnCooldown = UpdateSpawnCooldown();
        }
        else
        {
            _localTime += deltaTime;
        }
    }

    private void SpawnEnemies()
    {
        if (_currentEnemyIndex < _enemyQueue.Count())
        {
            if (_enemies.Count < _enemyQueue.MaxEnemiesOnScreen)
            {
                if(Array.Exists(BOSS_TYPES, x => x == _enemyQueue.Get(_currentEnemyIndex)))
                {
                    EnemyBase enemy = SpawnBoss(_enemyQueue.Get(_currentEnemyIndex));
                    _enemies.Add(enemy);
                    BossSpawned?.Invoke(Boss);
                }
                else
                {
                    EnemyBase enemy = SpawnRegularEnemy(_enemyQueue.Get(_currentEnemyIndex));
                    _enemies.Add(enemy);
                }
                _currentEnemyIndex++;
                EnemiesAvailable--;
            }
            else
            {
                // Reset timer until there is enough room for the next enemy
                // Effectively setting the timer on pause
                _localTime = 0; 
            }
        }
        else
        {
            _isWaveActive = false;
        }
    }

    private EnemyBase SpawnRegularEnemy(EnemyType enemyType)
    {
        var enemy = _enemyPool.Get(enemyType);
        enemy.Initialize();
        return enemy;
    }

    private EnemyBase SpawnBoss(EnemyType enemyType)
    {
        if (enemyType == EnemyType.Wasp)
        {
            Boss = _waspBoss;
        }
        else if (enemyType == EnemyType.Megamothling)
        {
            Boss = _megamothlingBoss;
        }
        else if (enemyType == EnemyType.Megabeetle)
        {
            Boss = _megabeetleBoss;
        }
        else if (enemyType == EnemyType.Dragonfly)
        {
            Boss = _dragonflyBoss;
        }
        return Boss;
    }

    private float UpdateSpawnCooldown()
    {
        float spawnDelayPhase = (float)(_currentEnemyIndex-1) / (_enemyQueue.Count()-1);
        float spawnDelayAcceleration = 1f/Mathf.Lerp(1, _enemyQueue.SpawnDelayAcceleration, spawnDelayPhase);
        return _enemyQueue.SpawnDelay * spawnDelayAcceleration;
    }
}
