using System;
using System.Collections;
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
    private WaitForSeconds _waitAfterGameOver = new (1f);
    private WaitForSeconds _waitToDeactivateEnemies;

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
    public event Action LampBlocked;
    public event Action LampUnblocked;
    
    public void Initialize()
    {
        Debug.Log("WaveEnemyDirector: Initializing");
        _spawnQueueGenerator = new SpawnQueueGenerator(_gameConfigService.SpawnQueueConfig.Data);
        _spawnQueue = _spawnQueueGenerator.Generate();
        _lampAttacker = new FLampAttacker();
        _enemyAttacker = new FEnemyAttacker(_gameConfigService.GameConfig.MaxAggressionLevel);
        
        _waitToDeactivateEnemies = new WaitForSeconds(_gameConfigService.GameConfig.GameoverInStageDuration);
        
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
    
    public void HandleLampDestroyed()
    {
        foreach (var enemy in _enemies)
        {
            if (enemy is IStickableWithLamp)
            {
                ((IStickableWithLamp)enemy).HandleLampDestroyed();
            }
        }
        _enemyAttacker.StopWave();
        StartCoroutine(SpreadEnemiesAfterGameOver());
        StartCoroutine(DeactivateEnemiesAfterGameOver());
        
        enabled = false;
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
                LampBlocked?.Invoke();
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
            LampUnblocked?.Invoke();
        }
    }
    //


    private IEnumerator SpreadEnemiesAfterGameOver()
    {
        yield return _waitAfterGameOver;
        SpreadEnemies();
    }
    
    private void SpreadEnemies()
    {
        foreach (var enemy in _enemies)
        {
            enemy.Spread();
        }
    }
    
    private IEnumerator DeactivateEnemiesAfterGameOver()
    {
        yield return _waitToDeactivateEnemies;
        ReturnAllActiveEnemiesToPool();
    }
    
    private void ReturnAllActiveEnemiesToPool()
    {
        Debug.Log("Return all enemies to pool CALLED");
        // Enemies
        if (_enemies.Count > 0)
        {
            foreach (var enemy in _enemies)
            {
                enemy.ReturnToPool();
            }            
        }
        
        // Bosses
        // if (_enemyAttacker.IsBossActive)
        // {
        //     _enemySpawner.Boss.Reset();
        // }
    }

    private void OnEnemyDead(FEnemy enemy)
    {
        // TODO: find better way to return enemies to pool that will work better with gameover one
        if (enabled)
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
