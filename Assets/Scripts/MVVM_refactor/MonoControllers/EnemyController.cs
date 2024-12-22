using System;
using System.Collections.Generic;
using UnityEngine;
// TODO: Remeake without MONOBHEAVIOR ???

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
    [SerializeField] private float _fireflyExplosionRadius;
    [SerializeField] private float _explosionDuration; // TODO: connect it to the FireflyExplosion component duration
    [Header("---- Waves Generation ------")]
    [SerializeField] private int _maxEnemiesOnScreen;
    [SerializeField] private int _agressionLevel;
    [SerializeField] private float _maxAggressionLevel;
    [Header("")]
    [SerializeField] private bool _isStartAtWaveTestMode = false;
    [SerializeField] private int _startAtWaveTest = 0;
    [SerializeField] private float _firstEnemySpawnDelay;
    
    private SpawnQueueGenerator _spawnQueueGenerator;
    private SpawnQueue _spawnQueue;
    
    private List<EnemyBase> _enemies;
    private List<EnemyBase> _enemiesReadyToAttack;
    private List<EnemyBase> _ladybugsPatrolling;

    private FEnemiesLampAttackHandler _enemiesLampAttackHandler;
    private EnemySpawner _enemySpawner;
    private EnemyAttacker _enemyAttacker;
    private EnemiesFireflyExploder _enemiesFireflyExploder;
    
    [SerializeField] private bool _isWaveInitialized = false;
    private bool _isGameActive = false;
    private int _enemiesKilled;
    
    // Dependencies    
    private IGameConfigService _gameConfigService;

    public void Construct(IGameConfigService gameConfigService)
    {
        _gameConfigService = gameConfigService;
    }
    
    // Events
    public event Action OnWaveStartEvent;
    public event Action<EnemyBase> OnEnemySpawnedEvent;
    public event Action<EnemyBase> OnEnemyDeadEvent;
    public event Action<EnemyBase> OnBossSpawnedEvent;
    public event Action<EnemyBase> OnBossDeadEvent;
    public event Action OnFireflyExplosionEvent;
    public event Action OnWaveEndEvent;
    
    private void OnEnable()
    {
        Enemy.OnEnemyDeactivatedEvent += UpdateEnemiesOnScreen;     // TODO: replace with an Interface
        Enemy.OnEnemyDeactivatedEvent += CheckForFireflyExplosion;  // TODO: replace with an Interface
        // Lamp.OnLampCollidedWithStickyEnemyEvent += UpdateLadybugsOnScreen;
        // BossBase.OnTriggerSpreadEvent += SpreadEnemies;
        // BossBase.OnDeathEvent += OnBossDeathHandle;
    }
    
    private void OnDisable()
    {
        Enemy.OnEnemyDeactivatedEvent -= UpdateEnemiesOnScreen;
        Enemy.OnEnemyDeactivatedEvent -= CheckForFireflyExplosion;
        // Lamp.OnLampCollidedWithStickyEnemyEvent -= UpdateLadybugsOnScreen;
        _enemySpawner.OnBossSpawnedEvent -= OnBossSpawnedHandle;
        // BossBase.OnTriggerSpreadEvent -= SpreadEnemies;
        // BossBase.OnDeathEvent -= OnBossDeathHandle;
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

        // Init all bosses
        _waspBoss.Initialize();
        _megamothlingBoss.Initialize();
        _megabeetleBoss.Initialize();
        _dragonflyBoss.Initialize();

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
        // And subcribe to its events
        _enemySpawner.OnBossSpawnedEvent += OnBossSpawnedHandle;
        
        _enemyAttacker = new EnemyAttacker(
            _spawnQueue, 
            _enemies, 
            _enemiesReadyToAttack, 
            _ladybugsPatrolling,
            _maxAggressionLevel
        );
        
        _enemiesFireflyExploder = new EnemiesFireflyExploder(
            _enemies, 
            _fireflyExplosion,
            _fireflyExplosionRadius,
            _explosionDuration
        );
        
        _isWaveInitialized = false;
        _isGameActive = true;
        
        // Debug Spawn Queue
        for(int i=0; i<_spawnQueue.Count(); i++)
        {
            Debug.Log($"Wave : {i}");
            string waveData = "";
            for(int j=0; j<_spawnQueue.Get(i).Count(); j++)
            {
                waveData = waveData + " - " + _spawnQueue.Get(i).Get(j).ToString();
            }
            Debug.Log(waveData);
        }
    }
    
    public void StartWave(int wave)
    {
        Debug.Log("Wave started");
        if(!_isWaveInitialized)
        {
            SetupWave(wave);
            OnWaveStartEvent?.Invoke(); // TODO: What we use this event for?
        }
    }
    
    private void SetupWave(int waveNum)
    {
        _enemySpawner.StartWave(waveNum);
        _enemyAttacker.StartWave(waveNum);
        _enemiesKilled = 0;
        _isWaveInitialized = true;
    }
    
    private void Update()
    {
        if (_isWaveInitialized && _isGameActive)
        {
            _enemySpawner.Tick();   
            _enemyAttacker.Tick();
            _enemiesFireflyExploder.Tick();
            
            if (_enemiesKilled == _enemySpawner.EnemiesWaveCount)
            {
                _isWaveInitialized = false;
                OnWaveEndEvent?.Invoke();
            }
        }
    }
    
    public void HandleAttackButtonClicked(float power)
    {
        Debug.Log("Enemy Manager Attack button clicked");
        // TODO: blocked attack support
        // we will use blocked bool as a parameter to have the only one method to call attack
        _enemiesLampAttackHandler.HandleLampAttack(_enemies, Converters.PowerToAttackPower(power));
        // OnWaveEndEvent?.Invoke();
    }
    
    // Event Handlers
    private void UpdateEnemiesOnScreen(EnemyBase enemy)
    {
        _enemies.Remove(enemy);
        _enemiesKilled++;
        if (enemy.EnemyType == EnemyType.Ladybug)
        {
            _ladybugsPatrolling.Remove(enemy);
        }
    }
    
    private void UpdateLadybugsOnScreen(EnemyBase enemy)
    {
        // Remove stick ladybug for damageable list
        if (enemy.EnemyType == EnemyType.Ladybug)
        {
            _ladybugsPatrolling.Remove(enemy);    
        }
    }
    
    private void CheckForFireflyExplosion(EnemyBase enemy)
    {
        if(enemy.EnemyType != EnemyType.Firefly)
        {
            return;
        }
        _enemiesFireflyExploder.StartExplosion(enemy);
        OnFireflyExplosionEvent?.Invoke();
    }
    
    private void OnBossSpawnedHandle(BossBase boss)
    {
        boss.Play();
        _enemyAttacker.ActivateBoss(boss); // Boss appearance should stop any ongoing attack
        OnBossSpawnedEvent?.Invoke(boss);
    }
}
