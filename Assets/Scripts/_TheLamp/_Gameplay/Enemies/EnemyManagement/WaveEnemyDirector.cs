using System;
using System.Collections.Generic;
using UnityEngine;

public class WaveEnemyDirector : MonoBehaviour, IInitializable
{
    private SpawnQueueGenerator _spawnQueueGenerator;
    private SpawnQueue _spawnQueue;
    private EnemyQueue _currentWaveEnemyQueue;
    private List<FEnemy> _enemies = new ();
    
    // Dependencies
    private IGameConfigService _gameConfigService;
    private FEnemySpawner _enemySpawner;
    private FEnemyAttacker _enemyAttacker;
    private LampStickyDetectionService _lampStickyDetectionService;
    
    public void Construct(
        IGameConfigService gameConfigService, 
        FEnemySpawner enemySpawner,
        FEnemyAttacker enemyAttacker,
        LampStickyDetectionService lampStickyDetectionService
        )
    {
        _gameConfigService = gameConfigService;
        _enemySpawner = enemySpawner;
        _enemyAttacker = enemyAttacker;
        _lampStickyDetectionService = lampStickyDetectionService;
    }
    
    public void Initialize()
    {
        Debug.Log("WaveEnemyDirector: Initializing");
        _spawnQueueGenerator = new SpawnQueueGenerator(_gameConfigService.SpawnQueueConfig.Data);
        _spawnQueue = _spawnQueueGenerator.Generate();
        
        _lampStickyDetectionService.AttackBlocked += _enemyAttacker.BlockAttackCooldown;
        _lampStickyDetectionService.AttackUnblocked += _enemyAttacker.UnblockAttackCooldown;
        _enemySpawner.EnemySpawned += OnEnemySpawned;
        
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

    private void OnDestroy()
    {
        _lampStickyDetectionService.AttackBlocked -= _enemyAttacker.BlockAttackCooldown;
        _lampStickyDetectionService.AttackUnblocked -= _enemyAttacker.UnblockAttackCooldown;
        _enemySpawner.EnemySpawned -= OnEnemySpawned;
    }

    private void OnEnemySpawned(FEnemy enemy)
    {
        if (enemy is IStickableWithLamp)
        {
            _lampStickyDetectionService.AddStickable((IStickableWithLamp)enemy);
        }
    }

    public void PrepareWave(int waveIndex)
    {
        _currentWaveEnemyQueue = _spawnQueue.Get(waveIndex);
        Debug.Log($"WaveEnemyDirector: Preparing Wave {waveIndex}");
        _enemySpawner.PrepareWave(_currentWaveEnemyQueue, _enemies);
        _enemyAttacker.PrepareWave(_currentWaveEnemyQueue.AggressionLevel, _enemies);
        
        // TODO: wave prepared switch on ????
        
    }
    
    public void StartWave()
    {
        _enemySpawner.StartWave();
        _enemyAttacker.StartWave();
    }
    
    public void StopWave()
    {
        _enemyAttacker.StopWave();
    }
}
