using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour,IInitializable
{
    [SerializeField] private SpawnQueueData _spawnQueueDataCache;
    
    [Header("------ Enemy Prefabs -------")]
    [SerializeField] private EnemyPool _enemyPool;
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
    [SerializeField] private int _startAtWave = 0;
    [SerializeField] private int _currentWave = 0;
    [SerializeField] private float _firstEnemySpawnDelay;

    [Header("---- Save Data ------")]
    [SerializeField] private SaveDataContainer _saveDataContainer;
    
    // Dependencies
    private IAnalyticsService _analyticsService;

    public int CurrentWave => _currentWave;

    private SpawnQueueGenerator _spawnQueueGenerator;
    private SpawnQueue _spawnQueue;
    
    private List<EnemyBase> _enemies;
    private List<EnemyBase> _enemiesReadyToAttack;
    private List<EnemyBase> _ladybugsPatrolling;

    private EnemiesLampAttackHandler _enemiesLampAttackHandler;
    private EnemySpawner _enemySpawner;
    private EnemyAttacker _enemyAttacker;
    private EnemiesFireflyExploder _enemiesFireflyExploder;

    private bool _isWaveInitialized = false;
    private bool _isGameActive = true;

    private float _attackDelay;
    private bool _isAttacking;

    private int _enemiesKilled;
    
    private WaitForSeconds _waitAfterGameOver = new WaitForSeconds(3.9f);
    
    public static event Action<int> OnWaveStartedEvent;
    public static event Action<int> OnWaveEndedEvent;
    public static event Action OnFireflyExplosionEvent;
    public static event Action OnEnemyDamagedEvent; 
    public static event Action<EnemyBase> OnBossAppearEvent;
    public static event Action<EnemyBase> OnBossDeathEvent;

    public void Inject(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }
    
    private void OnEnable()
    {
        Enemy.OnEnemyDeactivatedEvent += UpdateEnemiesOnScreen;
        Enemy.OnEnemyDeactivatedEvent += CheckForFireflyExplosion;
        LampAttackModel.OnLampAttackEvent += LampAttack;
        LampAttackModel.OnLampBlockedAttackEvent += LampBlockedAttack;
        Lamp.OnLampCollidedWithStickyEnemyEvent += UpdateLadybugsOnScreen;
        BossBase.OnTriggerSpreadEvent += SpreadEnemies;
        BossBase.OnDeathEvent += OnBossDeathHandle;
    }
    
    private void OnDisable()
    {
        Enemy.OnEnemyDeactivatedEvent -= UpdateEnemiesOnScreen;
        Enemy.OnEnemyDeactivatedEvent -= CheckForFireflyExplosion;
        LampAttackModel.OnLampAttackEvent -= LampAttack;
        LampAttackModel.OnLampBlockedAttackEvent -= LampBlockedAttack;
        Lamp.OnLampCollidedWithStickyEnemyEvent -= UpdateLadybugsOnScreen;
        _enemySpawner.OnBossSpawnedEvent -= OnBossSpawnedHandle;
        BossBase.OnTriggerSpreadEvent -= SpreadEnemies;
        BossBase.OnDeathEvent -= OnBossDeathHandle;
    }
    
    public void Initialize()
    {
        _isGameActive = true;
        _enemyPool.Initialize();
        _spawnQueueGenerator = new SpawnQueueGenerator(_spawnQueueDataCache.Data);
        _spawnQueue = _spawnQueueGenerator.Generate();
        _enemies = new List<EnemyBase>();
        _ladybugsPatrolling = new List<EnemyBase>();
        _enemiesReadyToAttack = new List<EnemyBase>();
        _enemiesLampAttackHandler = new EnemiesLampAttackHandler();
        
        // Load Game State Data
        if (!_isStartAtWaveTestMode)
        {
            _startAtWave = _saveDataContainer.Wave;    
        }
        
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
        
        _currentWave = _startAtWave;
        _isWaveInitialized = false;
    }

    public void StartWave()
    {
        if (!_isWaveInitialized)
        {
            SetupWave(_currentWave);
            _analyticsService.SubmitWaveStartEvent(_currentWave);
            OnWaveStartedEvent?.Invoke(_currentWave);
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
            _enemySpawner.Boss.SetGameover();
        }
    }
    
    public void DeactivateAllEnemies()
    {
        ReturnAllActiveEnemiesToPool();    
    }
    
    private IEnumerator SpreadEnemiesAfterGameOver()
    {
        yield return _waitAfterGameOver;
        SpreadEnemies();
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
        
        _currentWave = _startAtWave;
        _isWaveInitialized = false;
    }
    
    private void SetupWave(int waveNum)
    {
        _enemySpawner.StartWave(waveNum);
        _enemyAttacker.StartWave(waveNum);

        _enemiesKilled = 0;
        _isWaveInitialized = true;
    }

    private void OnBossSpawnedHandle(BossBase boss)
    {
        boss.Play();
        _enemyAttacker.ActivateBoss(boss); // Boss appearance should stop any ongoing attack
        OnBossAppearEvent?.Invoke(boss);
    }
    
    private void OnBossDeathHandle()
    {
        OnBossDeathEvent?.Invoke(_enemySpawner.Boss);
        _enemyAttacker.DeactivateBoss();
        _enemies.Remove(_enemySpawner.Boss);
        _enemiesKilled++;
    }
    
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
    
    // Handle Lamp Attacks
    private void LampAttack(int attackPower, float currentPower, float attackDuration, float attackDistance)
    {
        _enemiesLampAttackHandler.HandleLampAttack(attackPower, currentPower, attackDuration, attackDistance, _enemies);
    }

    private void LampBlockedAttack(int attackPower, float currentPower, float attackDuration, float attackDistance)
    {
        _enemiesLampAttackHandler.HandleLampBlockedAttack(attackPower, currentPower, attackDuration, attackDistance, _enemies);
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
    
    private void SpreadEnemies()
    {
        foreach (var enemy in _enemies)
        {
            enemy.SpreadStart();
        }
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
            _enemySpawner.Tick();   
            _enemyAttacker.Tick();
            _enemiesFireflyExploder.Tick();
            
            if (_enemiesKilled == _enemySpawner.EnemiesWaveCount)
            {
                _isWaveInitialized = false;
                _currentWave++;
                _saveDataContainer.Wave = _currentWave;
                OnWaveEndedEvent?.Invoke(_currentWave);
            }
        }
    }

    // Event Handlers
    private void HandleOnEnemyDamaged()
    {
        OnEnemyDamagedEvent?.Invoke();
    }
}
