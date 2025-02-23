using System;
using System.Collections.Generic;
using UnityEngine;

public class WaveEnemyDirector : MonoBehaviour, IInitializable
{
    private SpawnQueueGenerator _spawnQueueGenerator;
    private SpawnQueue _spawnQueue;
    private EnemyQueue _currentWaveEnemyQueue;
    private List<FEnemy> _enemies = new ();
    private List<IStickableWithLamp> _stickedEnemies = new ();
    private int _enemiesKilledCount = 0;
    private FLampAttacker _lampAttacker;
    [SerializeField] private bool _lampBlocked = false;
    private FEnemyAttacker _enemyAttacker;

    // Dependencies
    private IGameConfigService _gameConfigService;
    private FEnemySpawner _enemySpawner;
    
    public void Construct(
        IGameConfigService gameConfigService, 
        FEnemySpawner enemySpawner
        )
    {
        _gameConfigService = gameConfigService;
        _enemySpawner = enemySpawner;
    }
    
    public event Action WaveEnded;
    public event Action<CollidableEnemy> EnemyAttackStarted;
    public event Action<IStickableWithLamp> StickyEnemySpawned;
    public event Action AttackBlocked;
    public event Action AttackUnblocked;
    
    public void Initialize()
    {
        Debug.Log("WaveEnemyDirector: Initializing");
        _spawnQueueGenerator = new SpawnQueueGenerator(_gameConfigService.SpawnQueueConfig.Data);
        _spawnQueue = _spawnQueueGenerator.Generate();
        _lampAttacker = new FLampAttacker();
        _enemyAttacker = new FEnemyAttacker(_gameConfigService.GameConfig.MaxAggressionLevel);
        
        _enemySpawner.EnemySpawned += OnEnemySpawned;
        _enemySpawner.EnemyReturnedToPool += OnEnemyDead;
        _enemyAttacker.EnemyAttackStarted += OnEnemyAttackStarted;
        
        enabled = false;
        
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
        _enemySpawner.EnemySpawned -= OnEnemySpawned;
        _enemySpawner.EnemyReturnedToPool -= OnEnemyDead;
        _enemyAttacker.EnemyAttackStarted -= OnEnemyAttackStarted;
    }

    public void PrepareWave(int waveIndex)
    {
        _currentWaveEnemyQueue = _spawnQueue.Get(waveIndex);
        Debug.Log($"WaveEnemyDirector: Preparing Wave {waveIndex}");
        _enemySpawner.PrepareWave(_currentWaveEnemyQueue, _enemies);
        _enemyAttacker.PrepareWave(_currentWaveEnemyQueue.AggressionLevel, _enemies);
        _stickedEnemies.Clear();
        _lampAttacker.SetUnBlockedMode();
        
        // TODO: wave prepared switch on ????
        
    }

    public void StartWave()
    {
        _lampBlocked = false;
        _enemiesKilledCount = 0;
        _enemySpawner.StartWave();
        _enemyAttacker.StartWave();
        enabled = true;
    }

    private void StopWave()
    {
        _enemyAttacker.StopWave();
        enabled = false;
        WaveEnded?.Invoke();
    }

    public void HandleAttackButtonClicked(float power)
    {
        _lampAttacker.Attack(power, _enemies);
    }

    // Calls from PlayerEnemyInteractionMediator
    public void BlockAttackCooldown()
    {
        _enemyAttacker.BlockAttackCooldown();
    }

    public void UnblockAttackCooldown()
    {
        _enemyAttacker.UnblockAttackCooldown();
    }

    public void AddStickyEnemy(IStickableWithLamp enemy)
    {
        if (!_stickedEnemies.Contains(enemy))
        {
            _stickedEnemies.Add(enemy);
            if (!_lampBlocked)
            {
                _lampBlocked = true;
                _lampAttacker.SetBlockedMode();
            }
        }
    }

    public void RemoveStickyEnemy(IStickableWithLamp enemy)
    {
        if (_stickedEnemies.Contains(enemy))
        {
            _stickedEnemies.Remove(enemy);
        }
        if (_stickedEnemies.Count == 0)
        {
            _lampBlocked = false;
            _lampAttacker.SetUnBlockedMode();
        }
    }
    //

    private void OnEnemyDead(FEnemy enemy)
    {
        if (_enemies.Contains(enemy))
        {
            _enemies.Remove(enemy);
            _enemiesKilledCount++;

            if (_enemiesKilledCount == _currentWaveEnemyQueue.Count())
            {
                StopWave();
            }
        }
        else
        {
            Debug.LogError("WaveEnemyDirector: Enemy not found in list");
        }    
    }

    private void OnEnemySpawned(FEnemy enemy)
    {
        if (enemy is IStickableWithLamp)
        {
            StickyEnemySpawned?.Invoke((IStickableWithLamp)enemy);
        }
    }

    private void OnEnemyAttackStarted(CollidableEnemy enemy)
    {
        // TODO: add to damageables
        EnemyAttackStarted?.Invoke(enemy);
    }

    private void Update()
    {
        // TODO: enable -disable when not needed (between waves)
        _enemyAttacker.Tick(Time.deltaTime);
    }
}
