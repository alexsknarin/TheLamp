using System;
using System.Collections.Generic;
using UnityEngine;

public class FEnemySpawner: ITickable, IDisposable
{
    private EnemyQueue _enemyQueue;
    private float _firstEnemySpawnDelay = 0.5f; // TODO: move to config
    private List<FEnemy> _activeEnemies;

    private int _currentEnemyIndex;
    private float _spawnCooldown;
    private float _localTime;
    private bool _isWaveActive = false;
    
    // Dependencies
    private readonly FEnemyPool _enemyPool;
    
    public FEnemySpawner(FEnemyPool enemyPool)
    {
        _enemyPool = enemyPool;
    }
    
    public event Action<FEnemy> EnemySpawned;
    public event Action<FEnemy> EnemyReleased;

    public void Initialize()
    {
        _enemyPool.EnemyReleased += OnEnemyReleased;
    }

    public void Dispose()
    {
        _enemyPool.EnemyReleased -= OnEnemyReleased;
    }

    private void OnEnemyReleased(FEnemy enemy)
    {
        EnemyReleased?.Invoke(enemy);
    }

    public void PrepareWave(EnemyQueue enemyQueue, List<FEnemy> enemies)
    {
        _enemyQueue = enemyQueue;
        _activeEnemies = enemies;
        
        string waveData = "";
        // TODO: Optimize preload 
        for (int i = 0; i < _enemyQueue.Count(); i++)
        {
            _enemyPool.PreloadEnemy(EnemyTypeLibrary.EnemyTypeDictionary[_enemyQueue.Get(i)]);
            waveData = waveData + " - " + _enemyQueue.Get(i).ToString();
        }
        Debug.Log(waveData);
    }

    public void StartWave()
    {
        _currentEnemyIndex = 0;
        _localTime = 0;
        _spawnCooldown = _firstEnemySpawnDelay;
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
            if (_activeEnemies.Count < _enemyQueue.MaxEnemiesOnScreen)
            {
                // Potential boss spawn here
                var enemy = SpawnRegularEnemy(_enemyQueue.Get(_currentEnemyIndex));
                _activeEnemies.Add(enemy);
                EnemySpawned?.Invoke(enemy);
                // TODO: perhaps need to update enemies count from the outside and remove the reference to _enemies list
                
                _currentEnemyIndex++;
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
            // Stop Spawning
            _isWaveActive = false;
        }
    }
    
    private FEnemy SpawnRegularEnemy(EnemyType enemyType)
    {
        var enemy = _enemyPool.Get(EnemyTypeLibrary.EnemyTypeDictionary[enemyType]);
        enemy.Play();
        return enemy;
    }
    
    private float UpdateSpawnCooldown()
    {
        float spawnDelayPhase = (float)(_currentEnemyIndex-1) / (_enemyQueue.Count()-1);
        float spawnDelayAcceleration = 1f/Mathf.Lerp(1, _enemyQueue.SpawnDelayAcceleration, spawnDelayPhase);
        return _enemyQueue.SpawnDelay * spawnDelayAcceleration;
    }
}
