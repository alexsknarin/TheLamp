using System;
using System.Collections.Generic;
using UnityEngine;

public class WaveEnemyDirector : MonoBehaviour, IInitializable
{
    private SpawnQueueGenerator _spawnQueueGenerator;
    private SpawnQueue _spawnQueue;
    private EnemyQueue _currentWaveEnemyQueue;
    private List<ITickable> _tickables = new();
    private List<FEnemy> _enemies = new ();
    
    // Dependencies
    private IGameConfigService _gameConfigService;
    private FEnemySpawner _enemySpawner;
    
    public void Construct(IGameConfigService gameConfigService, FEnemySpawner enemySpawner)
    {
        _gameConfigService = gameConfigService;
        _enemySpawner = enemySpawner;
    }
    
    public void Initialize()
    {
        Debug.Log("WaveEnemyDirector: Initializing");
        _spawnQueueGenerator = new SpawnQueueGenerator(_gameConfigService.SpawnQueueConfig.Data);
        _spawnQueue = _spawnQueueGenerator.Generate();
        
        _tickables.Add(_enemySpawner);
        
        // Debug Spawn Queue
        // Debug.Log("++++ ----- Spawn Queue Generated:");
        // for(int i=0; i<_spawnQueue.Count(); i++)
        // {
        //     Debug.Log($"Wave : {i} --- Count: {_spawnQueue.Get(i).Count()}");
        //     string waveData = "";
        //     for(int j=0; j<_spawnQueue.Get(i).Count(); j++)
        //     {
        //         waveData = waveData + " - " + _spawnQueue.Get(i).Get(j).ToString();
        //     }
        //     Debug.Log(waveData);
        // }
    }
    
    public void PrepareWave(int waveIndex)
    {
        _currentWaveEnemyQueue = _spawnQueue.Get(waveIndex);
        Debug.Log($"WaveEnemyDirector: Preparing Wave {waveIndex}");
        _enemySpawner.PrepareWave(_currentWaveEnemyQueue, _enemies);
        
        // TODO: wave prepared switch on ????
        
    }
    
    public void StartWave()
    {
        _enemySpawner.StartWave();
        enabled = true; // TODO: disable when on the wave end
    }

    private void Update()
    {
        foreach (var tickable in _tickables)
        {
            tickable.Tick(Time.deltaTime);
        }
    }
}
