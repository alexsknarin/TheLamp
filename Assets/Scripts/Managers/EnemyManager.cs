using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using Random = UnityEngine.Random;

public class EnemyManager : MonoBehaviour,IInitializable
{
    [SerializeField] private SpawnQueueData _spawnQueueDataCache;
    private SpawnQueueGenerator _spawnQueueGenerator;
    private SpawnQueue _spawnQueue;
    
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
    [SerializeField] private float _explosionDuration;
    private EnemyBase _explosionSource;
    private Vector3 _explosionPosition;
    private bool _isExplosionActive = false;
    
    [Header("---- Waves Generation ------")]
    [SerializeField] private int _maxEnemiesOnScreen;
    [SerializeField] private int _agressionLevel;
    [SerializeField] private float _maxAggressionLevel;
    
    [Header("")]
    [SerializeField] private bool _isStartAtWaveTestMode = false;
    [SerializeField] private int _startAtWave = 0;
    [SerializeField] private int _currentWave = 0;
    [SerializeField] private float _firstEnemySpawnDelay;
    public int CurrentWave => _currentWave;
    private int _enemiesKilled;
    
    [Header("---- Debug ------")]
    [SerializeField] private int _enemiesInWaveCount; // Debug
    [SerializeField] private int _enemiesLeftUnspawnedCount; // Debug
    private List<EnemyBase> _enemies;
    private List<EnemyBase> _enemiesReadyToAttack;
    private List<EnemyBase> _ladybugsPatrolling;
    private EnemiesLampAttackHandler _enemiesLampAttackHandler;
    private EnemiesExplosionHandler _enemiesExplosionHandler;
    
    [Header("---- Save Data ------")]
    [SerializeField] private SaveDataContainer _saveDataContainer;

    private bool _isWaveInitialized = false;
    private bool _isGameActive = true;
    
    private float _attackDelay;
    private float _attackLocalTime;
    private bool _isAttacking;
    
    private float _explosionLocalTime;
    
    private bool _isBossActive = false;
    
    private WaitForSeconds _waitAfterGameOver = new WaitForSeconds(3.9f);
    
    
    public static event Action<int> OnWaveStartedEvent;
    public static event Action<int> OnWaveEndedEvent;
    public static event Action OnFireflyExplosionEvent;
    public static event Action OnEnemyDamagedEvent; 
    public static event Action<EnemyBase> OnBossAppearEvent;
    public static event Action<EnemyBase> OnBossDeathEvent;

    /// ------
    private EnemySpawner _enemySpawner;
    
    private void OnEnable()
    {
        Enemy.OnEnemyDeactivatedEvent += UpdateEnemiesOnScreen;
        Enemy.OnEnemyDeactivatedEvent += StartExplodeEnemyOnDeath;
        LampAttackModel.OnLampAttackEvent += LampAttack;
        LampAttackModel.OnLampBlockedAttackEvent += LampBlockedAttack;
        Lamp.OnLampCollidedWithStickyEnemyEvent += UpdateLadybugsOnScreen;
        BossBase.OnTriggerSpreadEvent += SpreadEnemies;
        BossBase.OnDeathEvent += HandleBossEnd;
    }
    
    private void OnDisable()
    {
        Enemy.OnEnemyDeactivatedEvent -= UpdateEnemiesOnScreen;
        Enemy.OnEnemyDeactivatedEvent -= StartExplodeEnemyOnDeath;
        LampAttackModel.OnLampAttackEvent -= LampAttack;
        LampAttackModel.OnLampBlockedAttackEvent -= LampBlockedAttack;
        Lamp.OnLampCollidedWithStickyEnemyEvent -= UpdateLadybugsOnScreen;
        _enemySpawner.OnBossSpawnedEvent -= OnBossSpawnedHandle;
        BossBase.OnTriggerSpreadEvent -= SpreadEnemies;
        BossBase.OnDeathEvent -= HandleBossEnd;
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
        _enemiesExplosionHandler = new EnemiesExplosionHandler();
        
        // Load Game State Data
        if (!_isStartAtWaveTestMode)
        {
            _startAtWave = _saveDataContainer.Wave;    
        }
        
        // Init all bosses
        _isBossActive = false;
        _waspBoss.Initialize();
        _megamothlingBoss.Initialize();
        _megabeetleBoss.Initialize();
        _dragonflyBoss.Initialize();


        // Create enemy spawner
        _enemySpawner = new EnemySpawner
        (
            _spawnQueue, 
            _enemies, 
            _enemyPool,
            _waspBoss,
            _megamothlingBoss,
            _megabeetleBoss,
            _dragonflyBoss,
            _maxAggressionLevel, 
            _firstEnemySpawnDelay
        );
        // And subcribe to its events
        _enemySpawner.OnBossSpawnedEvent += OnBossSpawnedHandle;
        
        
        _currentWave = _startAtWave;
        _isWaveInitialized = false;
    }

    public void StartWave()
    {
        if (!_isWaveInitialized)
        {
            SetupWave(_currentWave);
            OnWaveStartedEvent?.Invoke(_currentWave);
        }
    }
    
    public void HandleGameOver()
    {
        _isGameActive = false;
        // Wait for 5 seconds, call enemies to spread and the return them all to the pool
        StartCoroutine(SpreadEnemiesAfterGameOver());
        // Disable boss
        if (_isBossActive)
        {
            _enemySpawner.Boss.IsGameOver = true;
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
        _isBossActive = false;
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

        _maxEnemiesOnScreen = _enemySpawner.MaxEnemiesOnScreen; // TODO: for debug only ????
        _agressionLevel = _enemySpawner.AgressionLevel; // TODO: for debug only ????

        // Init Attack
        _attackDelay = GetRandomAttackDelay(4.5f, 1.8f, 6.5f, 2.8f, _enemySpawner.AggressionLevelNormalized);
        _enemiesKilled = 0;
        _attackLocalTime = 0;
        
        _isWaveInitialized = true;
        
        // Debug
        _enemiesInWaveCount = _enemySpawner.EnemiesWaveCount;
        _enemiesLeftUnspawnedCount = _enemySpawner.EnemiesAvailable;
    }

    private void OnBossSpawnedHandle(BossBase boss)
    {
        boss.Play();
        _isBossActive = true;
        _attackLocalTime = 0;
        OnBossAppearEvent?.Invoke(boss);
    }
    
    private void LampAttack(int attackPower, float currentPower, float attackDuration, float attackDistance)
    {
        _enemiesLampAttackHandler.HandleLampAttack(attackPower, currentPower, attackDuration, attackDistance, _enemies);
    }
    
    private void LampBlockedAttack(int attackPower, float currentPower, float attackDuration, float attackDistance)
    {
        _enemiesLampAttackHandler.HandleLampBlockedAttack(attackPower, currentPower, attackDuration, attackDistance, _enemies);
    }
    
    private void UpdateEnemiesOnScreen(EnemyBase enemy)
    {
        _enemies.Remove(enemy);
        _enemiesKilled++;
        if (enemy.EnemyType == EnemyTypes.Ladybug)
        {
            _ladybugsPatrolling.Remove(enemy);
        }
    }
    
    private void UpdateLadybugsOnScreen(EnemyBase enemy)
    {
        // Remove stick ladybug for damageable list
        if (enemy.EnemyType == EnemyTypes.Ladybug)
        {
            _ladybugsPatrolling.Remove(enemy);    
        }
    }

    private void StartExplodeEnemyOnDeath(EnemyBase explosionSource)
    {
        if(explosionSource.EnemyType != EnemyTypes.Firefly)
        {
            return;
        }
        _explosionSource = explosionSource;
        _explosionPosition = explosionSource.transform.position;
        _fireflyExplosion.Play(_explosionPosition, _fireflyExplosionRadius * 2);
        OnFireflyExplosionEvent?.Invoke();
        _explosionLocalTime = 0;
        _isExplosionActive = true;
    }
    
    private void SpreadEnemies()
    {
        foreach (var enemy in _enemies)
        {
            enemy.SpreadStart();
        }
    }

    private void HandleBossEnd()
    {
        OnBossDeathEvent?.Invoke(_enemySpawner.Boss);
        _isBossActive = false;
        _enemies.Remove(_enemySpawner.Boss);
        _enemiesKilled++;
    }

    private void Update()
    {

        if (_isWaveInitialized && _isGameActive)
        {
            _enemySpawner.Tick();   
            
            UpdateEnemiesReadyToAttack(_enemiesReadyToAttack, _enemies);

            if (!_isAttacking)
            {
                DelayAttack();
            }

            if (_enemiesReadyToAttack.Count > 0)
            {
                if (_isAttacking == true)
                {
                    StartEnemyAttack();
                }
            }

            if (_isExplosionActive)
            {
                float explosionPhase = _explosionLocalTime / _explosionDuration;
                if (explosionPhase > 1)
                {
                    _isExplosionActive = false;
                }
                else
                {
                    _enemiesExplosionHandler.HandleExplosion(_enemies, _explosionSource, _explosionPosition, _fireflyExplosionRadius);
                }
            }
            
            
            if (_enemiesKilled == _enemySpawner.EnemiesWaveCount)
            {
                _isWaveInitialized = false;
                _currentWave++;
                _saveDataContainer.Wave = _currentWave;
                OnWaveEndedEvent?.Invoke(_currentWave);
                return;
            }
            
            
            // Check if Ladybugs are blocking other enemies from attacking
            bool isAttackTimerUpdateAllowed = CheckIfAttackTimeUpdateIsAllowed(_ladybugsPatrolling.Count, _ladybugsPatrolling);
            
            UpdateTimers(ref _attackLocalTime, ref _explosionLocalTime, 
                isAttackTimerUpdateAllowed, _isBossActive);
        }
    }
    
    private void UpdateEnemiesReadyToAttack(List<EnemyBase> enemiesReadyToAttack, List<EnemyBase> enemies)
    {
        enemiesReadyToAttack.Clear();
        foreach (var enemy in enemies)
        {
            enemy.UpdateAttackAvailability();
            if (enemy.ReadyToAttack)
            {
                enemiesReadyToAttack.Add(enemy);
            }
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
        if (_isBossActive)
        {
            _enemySpawner.Boss.Reset();
        }
    }
    
    private void DelayAttack()
    {
        float attackPhase = _attackLocalTime / _attackDelay;
        if (attackPhase > 1)
        {
            _isAttacking = true;
        }   
    }
    
    private void StartEnemyAttack()
    {
        var attackingEnemy = _enemiesReadyToAttack[Random.Range(0, _enemiesReadyToAttack.Count)];

        // MEGAMOTHLING:
        // I attacks alongside other enemies but it should be
        // Priortized to attack more ofthen
        // IN this case twice as often
        
        if (_isBossActive && 
            (_enemySpawner.Boss.EnemyType == EnemyTypes.Megamothling) && 
            _enemySpawner.Boss.ReadyToAttack)
        {
            int megamothlingAttackChance = Random.Range(0, 2);
            if (megamothlingAttackChance == 0)
            {
                attackingEnemy = _enemySpawner.Boss;
            }
        }
       
        attackingEnemy.StartAttack();
        _attackLocalTime = 0;
        _attackDelay = GetRandomAttackDelay(2.5f, 0.8f, 6.1f, 1.8f, _enemySpawner.AggressionLevelNormalized);
        _isAttacking = false;
    }
    
    private bool CheckIfAttackTimeUpdateIsAllowed(int ladybugPatrollingCount, List<EnemyBase> ladybugsPatrolling)
    {
        if (ladybugPatrollingCount > 0)
        {
            foreach (var ladybug in ladybugsPatrolling)
            {
                Vector3 pos = ladybug.transform.position;
                pos.z = 0;
                // if any Ladybug is at 0.87f distance or close - no enemy attack is allowed
                if (pos.magnitude < 0.87f || ladybug.IsAttacking)
                {
                    return false;
                }
            }
        }
        return true;
    }
    
    private void UpdateTimers(ref float attackLocalTime, ref float explosionLocalTime, 
        bool isAttackUpdateAllowed, bool isBossActive)
    {
        if(isAttackUpdateAllowed && 
           (!isBossActive || 
            (_enemySpawner.Boss.EnemyType == EnemyTypes.Megamothling ||
             _enemySpawner.Boss.EnemyType == EnemyTypes.Megabeetle || 
             _enemySpawner.Boss.EnemyType == EnemyTypes.Dragonfly)))
        {
            attackLocalTime += Time.deltaTime;
        }
        explosionLocalTime += Time.deltaTime;
    }

    private float GetRandomAttackDelay(float minMin, float minMax, float maxMin, float maxMax, float aggressionLevel)
    {
        return Random.Range(
            Mathf.Lerp(minMin, minMax, aggressionLevel),
            Mathf.Lerp(maxMin, maxMax, aggressionLevel)
        );
    }
    
    // Event Handlers
    private void HandleOnEnemyDamaged()
    {
        OnEnemyDamagedEvent?.Invoke();
    }
}
