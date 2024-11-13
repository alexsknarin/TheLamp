using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner
{
    // Dependencies
    private readonly SpawnQueue _spawnQueue;
    private readonly float _maxAggressionLevel;
    private float _firstEnemySpawnDelay;
    private List<int> _enemies = new List<int>(); // TODO: replace with actual enemy objects from Enemy Manager
    
    private EnemyQueue _enemyQueue;
    private int _currentWave;
    private int _currentEnemyIndex;
    
    private float _spawnCooldown;
    private float _aggressionLevelNormalized;
    
    private float _localTime;
    private bool _isWaveActive = false;
    
    public EnemySpawner(
        SpawnQueue spawnQueue, 
        float maxAggressionLevel,
        float firstEnemySpawnDelay
        )
    {
        _spawnQueue = spawnQueue;
        _maxAggressionLevel = maxAggressionLevel;
        _firstEnemySpawnDelay = firstEnemySpawnDelay;
    }
    
    public void StartWave(int wave)
    {
        _currentWave = wave;
        _enemyQueue = _spawnQueue.Get(wave);
        Debug.Log("---------------------");
        Debug.Log("Starting wave " + wave);
        Debug.Log("Max enemies on screen: " + _enemyQueue.MaxEnemiesOnScreen);
        Debug.Log("Aggression level: " + _enemyQueue.AggressionLevel);
        Debug.Log("Spawn delay: " + _enemyQueue.SpawnDelay);
        Debug.Log("Spawn delay acceleration: " + _enemyQueue.SpawnDelayAcceleration);
        Debug.Log("- Enemy types -");
        for (int i = 0; i < _enemyQueue.Count(); i++)
        {
            Debug.Log(_enemyQueue.Get(i));
        }
        Debug.Log("---------------------");
        
        // Start spawning enemies
        _aggressionLevelNormalized = _enemyQueue.AggressionLevel / _maxAggressionLevel;
        _spawnCooldown = _firstEnemySpawnDelay;
        _localTime = 0;
        _currentEnemyIndex = 0;
        _isWaveActive = true;
    }

    private void WaitForCooldown()
    {
        if (_localTime >= _spawnCooldown)
        {
            _localTime = 0;
            SpawnEnemies();
            _spawnCooldown = UpdateSpawnCooldown();
        }
        else
        {
            _localTime += Time.deltaTime;
        }
    }

    private void SpawnEnemies()
    {
        if (_currentEnemyIndex < _enemyQueue.Count())
        {
            if (_enemies.Count < _enemyQueue.MaxEnemiesOnScreen) // TODO: check if it should be <=
            {
                Debug.Log("Spawning enemy " + _enemyQueue.Get(_currentEnemyIndex));
                Vector3 debugPos = new Vector3(_currentEnemyIndex+0.1f, 2, 0);
                Debug.DrawRay(Vector3.zero, debugPos, Color.cyan, 20f);
                _enemies.Add(_currentEnemyIndex);
                
                
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
            _isWaveActive = false;
            Debug.Log("Wave " + _currentWave + " has ended");
        }
    }

    private float UpdateSpawnCooldown()
    {
        float spawnDelayPhase = (float)(_currentEnemyIndex-1) / (_enemyQueue.Count()-1);
        float spawnDelayAcceleration = 1f/Mathf.Lerp(1, _enemyQueue.SpawnDelayAcceleration, spawnDelayPhase);
        return _enemyQueue.SpawnDelay * spawnDelayAcceleration;
    }


    public void Tick()
    {
        if (_isWaveActive)
        {
            WaitForCooldown();
        }
    }
    
    
    
}
