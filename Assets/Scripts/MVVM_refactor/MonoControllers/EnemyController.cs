using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Remeake without MONOBHEAVIOR ??? SO + Tickable - after bosses are made into prefabs

public class EnemyController : MonoBehaviour, IInitializable
{
    [Header("------ Enemy Prefabs -------")]
    [SerializeField] private EnemyPoolSO _enemyPool;
    [Header("------ Boss Prefabs -------")]
    [SerializeField] private BossBase _waspBoss;
    [SerializeField] private BossBase _megamothlingBoss;
    [SerializeField] private BossBase _megabeetleBoss;
    [SerializeField] private BossBase _dragonflyBoss;
    [Header("------ Explosions -------")]
    [SerializeField] private FireflyExplosion _fireflyExplosion;
    [Header("---- Waves Generation ------")]
    [SerializeField] private int _maxEnemiesOnScreen;
    [SerializeField] private int _agressionLevel;
    [SerializeField] private float _maxAggressionLevel;
    [Header("")]
    [SerializeField] private float _firstEnemySpawnDelay;
    [SerializeField] private bool _isWaveInitialized = false;
    private SpawnQueueGenerator _spawnQueueGenerator;
    private SpawnQueue _spawnQueue;
    private List<EnemyBase> _enemies;
    private List<EnemyBase> _enemiesReadyToAttack;
    // TODO: use interfaces to build these lists
    // Ldybugs in this list are used to check if some of them close enough to the player,
    // in this case all other enemies should stop attacking - TODO: refactor this
    private List<EnemyBase> _ladybugsPatrolling;
    private FEnemiesLampAttackHandler _enemiesLampAttackHandler;
    private EnemySpawner _enemySpawner;
    private EnemyAttacker _enemyAttacker;
    private EnemiesFireflyExploder _enemiesFireflyExploder;
    private List<ITickable> _tickables;
    private bool _isGameActive = false;
    private int _enemiesKilled;
    private bool _isPlayerBlocked = false;
    private WaitForSeconds _waitAfterGameOver = new WaitForSeconds(3.9f); // TODO: Magic Number
    // Dependencies    
    private IGameConfigService _gameConfigService;
    
    public void Construct(IGameConfigService gameConfigService)
    {
        _gameConfigService = gameConfigService;
    }
    
    // Events
    public event Action WaveStarted;
    public event Action<EnemyBase> EnemySpawned;
    public event Action<EnemyBase> EnemyDied;
    public event Action<EnemyBase> BossSpawned;
    public event Action<EnemyBase> BossDied;
    public event Action FireflyExplosionStarted;
    public event Action WaveEnded;
    
    private void OnEnable()
    {
        Enemy.EnemyDeactivated += OnEnemyDeactivated;     // TODO: replace with an Interface
        Enemy.EnemyDeactivated += CheckForFireflyExplosion;  // TODO: replace with an Interface // TODO: call from a single method
        LampStickZoneCollisionHandler.CollidedWithStickyEnemyStatic += UpdateLadybugsOnScreen; // TODO: tmp solution - need fix
        BossBase.SpreadTriggering += OnSpreadTriggering;
        BossBase.BossDied += OnBossDied;
    }

    public void Initialize()
    {
        _spawnQueueGenerator = new SpawnQueueGenerator(_gameConfigService.SpawnQueueConfig.Data);
        _spawnQueue = _spawnQueueGenerator.Generate();
        
        // TODO: use interfaces to build these lists
        _enemyPool.Initialize();
        _enemies = new List<EnemyBase>();
        _ladybugsPatrolling = new List<EnemyBase>();
        _enemiesReadyToAttack = new List<EnemyBase>();
        _enemiesLampAttackHandler = new FEnemiesLampAttackHandler();

        // // Init all bosses
        // _waspBoss.Initialize();
        // _megamothlingBoss.Initialize();
        // _megabeetleBoss.Initialize();
        // _dragonflyBoss.Initialize();

        _tickables = new List<ITickable>();
        // Create enemy spawner
        _enemySpawner = new EnemySpawner(
            _spawnQueue, 
            _enemies, 
            _enemyPool,
            _waspBoss,
            _megamothlingBoss,
            _megabeetleBoss,
            _dragonflyBoss,
            _firstEnemySpawnDelay
        );
        _tickables.Add(_enemySpawner);
        
        // And subcribe to its events
        _enemySpawner.BossSpawned += OnBossSpawned;
        
        _enemyAttacker = new EnemyAttacker(
            _spawnQueue, 
            _enemies, 
            _enemiesReadyToAttack, 
            _ladybugsPatrolling,
            _maxAggressionLevel
        );
        _tickables.Add(_enemyAttacker);
        
        _enemiesFireflyExploder = new EnemiesFireflyExploder(
            _enemies, 
            _fireflyExplosion,
            _gameConfigService.GameConfig.FireflyExplosionRadius,
            _gameConfigService.GameConfig.FireflyExplosionDuration
        );
        _tickables.Add(_enemiesFireflyExploder);
        
        _isWaveInitialized = false;
        _isGameActive = true;
        
        // Debug Spawn Queue
        for(int i=0; i<_spawnQueue.Count(); i++)
        {
            Debug.Log($"Wave : {i} --- Count: {_spawnQueue.Get(i).Count()}");
            string waveData = "";
            for(int j=0; j<_spawnQueue.Get(i).Count(); j++)
            {
                waveData = waveData + " - " + _spawnQueue.Get(i).Get(j).ToString();
            }
            Debug.Log(waveData);
        }
    }

    private void OnDisable()
    {
        Enemy.EnemyDeactivated -= OnEnemyDeactivated;
        Enemy.EnemyDeactivated -= CheckForFireflyExplosion;
        LampStickZoneCollisionHandler.CollidedWithStickyEnemyStatic -= UpdateLadybugsOnScreen;
        _enemySpawner.BossSpawned -= OnBossSpawned;
        BossBase.SpreadTriggering -= OnSpreadTriggering;
        BossBase.BossDied -= OnBossDied;
    }

    public void StartWave(int wave)
    {
        Debug.Log("Wave started");
        if(!_isWaveInitialized)
        {
            SetupWave(wave);
            WaveStarted?.Invoke(); // TODO: What we use this event for?
        }
    }

    public void HandleLampDestroyed()
    {
        foreach (var enemy in _enemies)
        {
            enemy.HandleLampDestroyed();
        }
    }

    public void HandleGameOver()
    {
        _isGameActive = false;
        // Wait for 5 seconds, call enemies to spread and the return them all to the pool
        StartCoroutine(SpreadEnemiesAfterGameOver());
        // Disable boss
        if (_enemyAttacker.IsBossActive)
        {
            _enemySpawner.Boss.SetGameOver();
        }
    }

    public void Restart()
    {
        ReturnAllActiveEnemiesToPool();
        _isGameActive = true;
        _spawnQueue = _spawnQueueGenerator.Generate();
        
        _enemies.Clear();
        _ladybugsPatrolling.Clear();
        _enemiesReadyToAttack.Clear();

        // Init all bosses
        _waspBoss.Initialize();
        _megamothlingBoss.Initialize();
        _megabeetleBoss.Initialize();
        _dragonflyBoss.Initialize();
        
        _isWaveInitialized = false;
    }

    public void HandleAttackButtonClicked(float power)
    {
        // TODO: blocked attack support
        // we will use blocked bool as a parameter to have the only one method to call attack
        
        int attackPower = Converters.PowerToAttackPower(power);
        if (attackPower > 0)
        {
            if (_isPlayerBlocked)
                _enemiesLampAttackHandler.HandleLampBlockedAttack(_enemies, attackPower);
            else
                _enemiesLampAttackHandler.HandleLampAttack(_enemies, attackPower);
        }
    }

    public void SetBlockedMode(bool isBlocked)
    {
        _isPlayerBlocked = isBlocked;
    }

    private IEnumerator SpreadEnemiesAfterGameOver()
    {
        yield return _waitAfterGameOver;
        OnSpreadTriggering();
    }

    private void SetupWave(int waveNum)
    {
        _enemySpawner.StartWave(waveNum);
        _enemyAttacker.StartWave(waveNum);
        _enemiesKilled = 0;
        _isWaveInitialized = true;
    }

    private void ReturnAllActiveEnemiesToPool()
    {
        // Enemies
        foreach (var enemy in _enemies)
        {
            enemy.ReturnToPool();
        }
        
        // Bosses
        if (_enemyAttacker.IsBossActive)
        {
            _enemySpawner.Boss.Reset();
        }
    }

    private void Update()
    {
        if (_isWaveInitialized && _isGameActive)
        {
            foreach (var tickable in _tickables)
            {
                tickable.Tick(Time.deltaTime);
            }
            
            if (_enemiesKilled == _enemySpawner.EnemiesWaveCount)
            {
                _isWaveInitialized = false;
                WaveEnded?.Invoke();
            }
        }
    }
    
    
    // Event Handle Methods
    /// <summary>
    /// Update enemies list
    /// </summary>
    /// <param name="enemy"></param>
    private void OnEnemyDeactivated(EnemyBase enemy)
    {
        _enemies.Remove(enemy);
        _enemiesKilled++;
        if (enemy.EnemyType == EnemyType.Ladybug) // TODO: Interfaces check interface instead of a type variable
        {
            _ladybugsPatrolling.Remove(enemy); // TODO: Interfaces
        }
    }

    private void CheckForFireflyExplosion(EnemyBase enemy)
    {
        if(enemy.EnemyType != EnemyType.Firefly)
        {
            return;
        }
        _enemiesFireflyExploder.StartExplosion(enemy);
        FireflyExplosionStarted?.Invoke();
    }

    private void UpdateLadybugsOnScreen(EnemyBase enemy)
    {
        // Remove stick ladybug for damageable list
        if (enemy.EnemyType == EnemyType.Ladybug) // TODO: Interfaces
        {
            _ladybugsPatrolling.Remove(enemy); // TODO: Interfaces
        }
    }

    private void OnBossSpawned(BossBase boss)
    {
        boss.Play();
        _enemyAttacker.ActivateBoss(boss); // Boss appearance should stop any ongoing attack
        BossSpawned?.Invoke(boss);
    }

    private void OnSpreadTriggering()
    {
        foreach (var enemy in _enemies)
        {
            enemy.SpreadStart();
        }
    }

    private void OnBossDied()
    {
        BossDied?.Invoke(_enemySpawner.Boss);
        _enemyAttacker.DeactivateBoss();
        _enemies.Remove(_enemySpawner.Boss);
        _enemiesKilled++;
    }
}
