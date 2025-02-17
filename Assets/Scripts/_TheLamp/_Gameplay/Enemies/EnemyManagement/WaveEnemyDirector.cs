using System;
using System.Collections.Generic;
using UnityEngine;

public class WaveEnemyDirector : MonoBehaviour, IInitializable
{
    // Dependencies
    private IGameConfigService _gameConfigService;
    private LampCollisionDetectionService _lampCollisionDetectionService;
    private FEnemySpawner _enemySpawner;
    
    private FEnemyAttacker _enemyAttacker;
    private SpawnQueueGenerator _spawnQueueGenerator;
    private SpawnQueue _spawnQueue;
    
    private List<ITickable> _tickables = new();
    private List<IDisposable> _disposables = new ();
    [SerializeField] private List<FEnemy> _enemies = new ();
    private List<IDamageable> _damageables = new();
    
    
    public void Construct(
        IGameConfigService gameConfigService,
        LampCollisionDetectionService lampCollisionDetectionService,
        FEnemySpawner enemySpawner)
    {
        _gameConfigService = gameConfigService;
        _lampCollisionDetectionService = lampCollisionDetectionService;
        _enemySpawner = enemySpawner;
    }

    public void Initialize()
    {
        _spawnQueueGenerator = new SpawnQueueGenerator(_gameConfigService.SpawnQueueConfig.Data);
        _enemySpawner.Initialize();
        _disposables.Add(_enemySpawner);
        _tickables.Add(_enemySpawner);
        _enemyAttacker = new FEnemyAttacker();
        _tickables.Add(_enemyAttacker);
        _enemyAttacker.EnemyAttackStarted += OnEnemyAttackStarted;
    }

    private void OnDestroy()
    {
        _enemyAttacker.EnemyAttackStarted -= OnEnemyAttackStarted;
        
        foreach (var disposable in _disposables)
        {
            disposable.Dispose();
        }
    }

    private void OnEnemyAttackStarted(CollidableEnemy enemy)
    {
        _lampCollisionDetectionService.AddCollidable(enemy);
        _damageables.Add(enemy);
    }

    public void HandleStartGame()
    {
        Debug.Log("Prepare mode for a game started");
        _spawnQueue = _spawnQueueGenerator.Generate();
    }

    public void PrepareWave(int waveNumber)
    {
        // Generate wave
        // preload enemy prefabs
        Debug.Log($"Prepare mode for a wave {waveNumber} started");
        Debug.Log($"Wave {waveNumber} generated");
        EnemyQueue enemyQueue = _spawnQueue.Get(waveNumber);
        
        for (int i = 0; i < enemyQueue.Count(); i++)
        {
            Debug.Log($"Wave: Enemy {enemyQueue.Get(i)} found");
        }
        
        _enemySpawner.PrepareWave(enemyQueue, _enemies);
        _enemyAttacker.PrepareWave(enemyQueue.AggressionLevel, _enemies);
        
    }

    public void StartWave()
    {
        _enemySpawner.StartWave();
        _enemyAttacker.StartWave();
    }
    
    private void Update()
    {
        foreach (var tickable in _tickables)
        {
            tickable.Tick(Time.deltaTime);
        }
        
        // Do Lamp Attack TODO: replace with external call
        if (Input.GetMouseButtonDown(0))
        {
            if (_damageables.Count != 0)
            {
                foreach (var damageable in _damageables)
                {
                    if (damageable.IsReadyForDamage)
                    {
                        damageable.ReceiveDamage(1);
                    }
                }
                
                _damageables.Clear();
            }
        }
    }
}
